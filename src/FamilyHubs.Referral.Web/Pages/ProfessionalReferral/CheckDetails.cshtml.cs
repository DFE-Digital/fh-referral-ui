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

    public CheckDetailsModel(
        IConnectionRequestDistributedCache connectionRequestCache,
        IOrganisationClientService organisationClientService,
        IReferralClientService referralClientService)
        : base(ConnectJourneyPage.CheckDetails, connectionRequestCache)
    {
        _organisationClientService = organisationClientService;
        _referralClientService = referralClientService;
    }

    protected override async Task OnGetWithModelAsync(ConnectionRequestModel model)
    {
        // do this now, so we don't display any previously entered contact details that are no longer selected
        // but don't remove them from the cache yet, in case the user goes back to change the contact details
        // we'll remove them from the cache when the user submits the form
        RemoveNonSelectedContactDetails(model);

        // if the user has gone to change details, errored on the page, then clicked back to here, we need to clear the error state, so that if they go back to the same details page it won't be errored
        model.ErrorState = null;
        await ConnectionRequestCache.SetAsync(ProfessionalUser.Email, model);
    }

    private static void RemoveNonSelectedContactDetails(ConnectionRequestModel model)
    {
        // remove any previously entered contact details that are no longer selected
        //todo: can we do this generically?
        if (!model.ContactMethodsSelected[(int) ConnectContactDetailsJourneyPage.Email])
        {
            model.EmailAddress = null;
        }

        if (!model.ContactMethodsSelected[(int) ConnectContactDetailsJourneyPage.Telephone])
        {
            model.TelephoneNumber = null;
        }

        if (!model.ContactMethodsSelected[(int) ConnectContactDetailsJourneyPage.Textphone])
        {
            model.TextphoneNumber = null;
        }

        if (!model.ContactMethodsSelected[(int) ConnectContactDetailsJourneyPage.Letter])
        {
            model.AddressLine1 = null;
            model.AddressLine2 = null;
            model.TownOrCity = null;
            model.County = null;
            model.Postcode = null;
        }
    }

    protected override async Task<IActionResult> OnPostWithModelAsync(ConnectionRequestModel model)
    {
        RemoveNonSelectedContactDetails(model);

        var requestNumber = await CreateConnectionRequest(model);

        return RedirectToPage("/ProfessionalReferral/Confirmation", new
        {
            ServiceId,
            requestNumber
        });
    }

    private async Task<string> CreateConnectionRequest(ConnectionRequestModel model)
    {
        //todo: this throws an ArgumentNullException if the service is not found. it should return null (from a 404 from the api)
        ServiceDto service = await _organisationClientService.GetLocalOfferById(model.ServiceId!);
        var organisation = await _organisationClientService.GetOrganisationDtobyIdAsync(service.OrganisationId);

        if (organisation == null)
        {
            //todo: create and throw custom exception
            throw new InvalidOperationException($"Organisation not found for service {service.Id}");
        }   

        //var team = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Team");

        var referralDto = CreateReferralDto(model, ProfessionalUser, /*team,*/ service, organisation);

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
            ReferrerDto = new ReferralUserAccountDto()
            {
                Id = long.Parse(user.AccountId),
                EmailAddress = user.Email,
                Name = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
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