using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Models;
using FamilyHubs.SharedKernel.Identity.Models;
using Microsoft.AspNetCore.Mvc;
using ReferralOrganisationDto = FamilyHubs.ReferralService.Shared.Dto.OrganisationDto;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class CheckDetailsModel : ProfessionalReferralCacheModel
{
    private readonly IReferralClientService _referralClientService;
    private readonly IReferralNotificationService _referralNotificationService;

    public CheckDetailsModel(
        IConnectionRequestDistributedCache connectionRequestCache,
        IReferralClientService referralClientService,
        IReferralNotificationService referralNotificationService)
        : base(ConnectJourneyPage.CheckDetails, connectionRequestCache)
    {
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

        var referralResponse = await CreateConnectionRequest(long.Parse(model.ServiceId!), model);

        await _referralNotificationService.OnCreateReferral(
            ProfessionalUser.Email, referralResponse.OrganisationId, referralResponse.ServiceName, referralResponse.Id);

        return RedirectToPage("/ProfessionalReferral/Confirmation", new { requestNumber = referralResponse.Id });
    }

    private async Task<ReferralResponse> CreateConnectionRequest(long serviceId, ConnectionRequestModel model)
    {
        var referralDto = CreateReferralDto(model, ProfessionalUser, serviceId);

        return await _referralClientService.CreateReferral(referralDto);
    }

    private static ReferralDto CreateReferralDto(
        ConnectionRequestModel model,
        FamilyHubsUser user,
        long serviceId)
    {
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
                Id = serviceId,
                //todo: make OrganisationDto not required
                OrganisationDto = new ReferralOrganisationDto()
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
}