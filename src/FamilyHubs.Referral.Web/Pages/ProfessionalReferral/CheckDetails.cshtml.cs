using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Extensions;
using FamilyHubs.SharedKernel.Identity.Models;
using Microsoft.AspNetCore.Mvc;
using SharedOrganisationDto = FamilyHubs.ServiceDirectory.Shared.Dto.OrganisationDto;
using ReferralOrganisationDto = FamilyHubs.ReferralService.Shared.Dto.OrganisationDto;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class CheckDetailsModel : ProfessionalReferralCacheModel
{
    private readonly IOrganisationClientService _organisationClientService;
    private readonly IReferralClientService _referralClientService;
    private readonly IReferralNotificationService _referralNotificationService;

    public CheckDetailsModel(
        IConnectionRequestDistributedCache connectionRequestCache,
        IOrganisationClientService organisationClientService,
        IReferralClientService referralClientService,
        IReferralNotificationService referralNotificationService)
        : base(ConnectJourneyPage.CheckDetails, connectionRequestCache)
    {
        _organisationClientService = organisationClientService;
        _referralClientService = referralClientService;
        _referralNotificationService = referralNotificationService;
    }

    protected override async Task OnGetWithModelAsync(ConnectionRequestModel model)
    {
        // do this now, so we don't display any previously entered contact details that are no longer selected
        // but don't remove them from the cache yet, in case the user goes back to change the contact details
        // we'll remove them from the cache when the user submits the form
        model.RemoveNonSelectedContactDetails();

        // if the user has gone to change details, errored on the page, then clicked back to here, we need to clear the error state, so that if they go back to the same details page it won't be errored
        model.ErrorState = null;
        await ConnectionRequestCache.SetAsync(ProfessionalUser.Email, model);
    }

    protected override async Task<IActionResult> OnPostWithModelAsync(ConnectionRequestModel model)
    {
        // remove any previously entered contact details that are no longer selected
        model.RemoveNonSelectedContactDetails();

        //todo: this throws an ArgumentNullException if the service is not found. it should return null (from a 404 from the api)
        var service = await _organisationClientService.GetLocalOfferById(model.ServiceId!);

        int requestNumber = await CreateConnectionRequest(service, model);

        await _referralNotificationService.OnCreateReferral(
            ProfessionalUser.Email, service.OrganisationId, service.Name, requestNumber);

        return RedirectToPage("/ProfessionalReferral/Confirmation", new { requestNumber });
    }

    private async Task<SharedOrganisationDto> GetOrganisation(ServiceDto service)
    {
        var organisation = await _organisationClientService.GetOrganisationDtobyIdAsync(service.OrganisationId);
        if (organisation == null)
        {
            //todo: create and throw custom exception
            throw new InvalidOperationException($"Organisation not found for service {service.Id}");
        }
        return organisation;
    }

    private async Task<int> CreateConnectionRequest(ServiceDto service, ConnectionRequestModel model)
    {
        var organisation = await GetOrganisation(service);

        //todo: should we check if the organisation is a VCFS organisation?
        //organisation.OrganisationType == OrganisationType.VCFS

        var referralDto = CreateReferralDto(model, ProfessionalUser, service, organisation);

        return await _referralClientService.CreateReferral(referralDto);
    }

    private static ReferralDto CreateReferralDto(
        ConnectionRequestModel model,
        FamilyHubsUser user,
        ServiceDto service,
        SharedOrganisationDto organisation)
    {
        var contact = service.GetContact();

        string? serviceWebsite = GetWebsiteUrl(contact?.Url);

        var referralDto = new ReferralDto
        {
            ReasonForSupport = model.Reason!,
            EngageWithFamily = model.EngageReason!,
            ReferrerTelephone = model.ReferrerTelephone,
            RecipientDto = new RecipientDto
            {
                Name = model.FamilyContactFullName!,
                Email = model.EmailAddress,
                Telephone = model.TelephoneNumber,
                TextPhone = model.TextphoneNumber,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                TownOrCity = model.TownOrCity,
                County = model.County,
                PostCode = model.Postcode
            },
            ReferralUserAccountDto = new UserAccountDto
            {
                Id = long.Parse(user.AccountId),
                EmailAddress = user.Email,
                Name = user.FullName,
                PhoneNumber = user.PhoneNumber,
                //todo: this would be better (less leaky and simpler)...
                //Role = user.Role,
                UserAccountRoles = new List<UserAccountRoleDto>
                {
                    new()
                    {
                        UserAccount = new UserAccountDto
                        {
                            EmailAddress = user.Email,
                        },
                        Role = new RoleDto
                        {
                            Name = user.Role
                        }
                    }
                }
            },
            ReferralServiceDto = new ReferralServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Url = serviceWebsite,
                OrganisationDto = new ReferralOrganisationDto
                {
                    Id = organisation.Id,
                    Name = organisation.Name,
                    Description = organisation.Description
                }
            },
            Status = new ReferralStatusDto
            {
                Name = "New"
            },
            Created = DateTime.UtcNow
        };
        referralDto.LastModified = referralDto.Created;
        return referralDto;
    }

    private static string? GetWebsiteUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return default;

        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            return url;

        // assume http! (UriBuilder interprets a single string as a host and insists on adding a '/' on the end, which doesn't work if the url contains query params)
        return $"http://{url}";
    }
}