using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Models;
using FamilyHubs.SharedKernel.Identity.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FamilyHubs.ReferralService.Shared.Dto.CreateUpdate;
using FamilyHubs.ReferralService.Shared.Dto.Metrics;
using ReferralOrganisationDto = FamilyHubs.ReferralService.Shared.Dto.OrganisationDto;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class CheckDetailsModel : ProfessionalReferralCacheModel
{
    private readonly IReferralClientService _referralClientService;
    private readonly IReferralNotificationService _referralNotificationService;

    public string ContactMethodDisplayNames { get; private set; } = null!;

    public string? Address { get; private set; }

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

        List<string> contactMethodDisplayNames = new();

        for (int i = 0; i < model.ContactMethodsSelected.Length; i++)
        {
            if (model.ContactMethodsSelected[i])
            {
                contactMethodDisplayNames.Add(ContactDetailsModel.StaticCheckboxes[i].Label);
            }
        }

        ContactMethodDisplayNames = string.Join(", ", contactMethodDisplayNames);

        // AddressLine1 is a proxy for the entire address as it is a mandatory field.
        if (!string.IsNullOrEmpty(model.AddressLine1))
        {
            Address = string.Join("<br>", RemoveEmpty(
                model.AddressLine1,
                model.AddressLine2,
                model.TownOrCity,
                model.County,
                model.Postcode
            ));
        }

        // if the user has gone to change details, errored on the page, then clicked back to here, we need to clear the error state, so that if they go back to the same details page it won't be errored
        model.ErrorState = null;
        await ConnectionRequestCache.SetAsync(ProfessionalUser.Email, model);
    }

    protected override async Task<IActionResult> OnPostWithModelAsync(ConnectionRequestModel model)
    {
        // remove any previously entered contact details that are no longer selected
        model.RemoveNonSelectedContactDetails();

        var requestTimestamp = DateTimeOffset.UtcNow;

        ReferralResponse? referralResponse = null;
        HttpStatusCode httpStatusCode;
        try
        {
            (referralResponse, httpStatusCode) =
                await CreateConnectionRequest(long.Parse(model.ServiceId!), model, requestTimestamp);
        }
        // false positives
#pragma warning disable S2583
        catch (ReferralClientServiceException e)
        {
            await UpdateConnectionRequestsSentMetric(requestTimestamp, referralResponse?.Id, e.StatusCode);
            throw;
        }
        catch
        {
            await UpdateConnectionRequestsSentMetric(requestTimestamp, referralResponse?.Id);
            throw;
        }
#pragma warning restore S2583

        await _referralNotificationService.OnCreateReferral(
            ProfessionalUser.Email, referralResponse.OrganisationId, referralResponse.ServiceName, referralResponse.Id);

        await UpdateConnectionRequestsSentMetric(requestTimestamp, referralResponse.Id, httpStatusCode);

        return RedirectToPage("/ProfessionalReferral/Confirmation", new { requestNumber = referralResponse.Id });
    }

    private Task UpdateConnectionRequestsSentMetric(
        DateTimeOffset requestTimestamp,
        long? referralResponseId,
        HttpStatusCode? httpStatusCode = null)
    {
        return _referralClientService.UpdateConnectionRequestsSentMetric(
            new UpdateConnectionRequestsSentMetricDto(requestTimestamp, httpStatusCode, referralResponseId));
    }

    private async Task<(ReferralResponse, HttpStatusCode)> CreateConnectionRequest(
        long serviceId,
        ConnectionRequestModel model,
        DateTimeOffset requestTimestamp)
    {
        var referralDto = CreateReferralDto(model, ProfessionalUser, serviceId);

        var metric = new ConnectionRequestsSentMetricDto(requestTimestamp);

        return await _referralClientService.CreateReferral(new CreateReferralDto(referralDto, metric));
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

    private static IEnumerable<string> RemoveEmpty(params string?[] list)
    {
        return list.Where(str => !string.IsNullOrWhiteSpace(str))!;
    }
}