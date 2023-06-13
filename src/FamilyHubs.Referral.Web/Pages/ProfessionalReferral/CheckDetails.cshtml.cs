using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.SharedKernel.Identity.Models;
using Microsoft.AspNetCore.Mvc;
using FamilyHubs.Referral.Core.Notifications;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class CheckDetailsModel : ProfessionalReferralCacheModel
{
    private readonly IOrganisationClientService _organisationClientService;
    private readonly IReferralClientService _referralClientService;
    private readonly INotifications _notifications;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CheckDetailsModel> _logger;

    public CheckDetailsModel(
        IConnectionRequestDistributedCache connectionRequestCache,
        IOrganisationClientService organisationClientService,
        IReferralClientService referralClientService,
        INotifications notifications,
        IConfiguration configuration,
        ILogger<CheckDetailsModel> logger)
        : base(ConnectJourneyPage.CheckDetails, connectionRequestCache)
    {
        _organisationClientService = organisationClientService;
        _referralClientService = referralClientService;
        _notifications = notifications;
        _configuration = configuration;
        _logger = logger;
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

        string requestNumber = await CreateConnectionRequest(service, model);

        try
        {
            //todo: VCS email, not professional
            await SendVcsNotificationEmail(ProfessionalUser.Email, requestNumber, service.Name);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Unable to send VcsNewRequest email for request {RequestNumber}", requestNumber);
            throw;
        }

        return RedirectToPage("/ProfessionalReferral/Confirmation", new { requestNumber });
    }

    private async Task<string> CreateConnectionRequest(
        ServiceDto service,
        ConnectionRequestModel model)
    {
        OrganisationDto? organisation = await _organisationClientService.GetOrganisationDtobyIdAsync(service.OrganisationId);

        if (organisation == null)
        {
            //todo: create and throw custom exception
            throw new InvalidOperationException($"Organisation not found for service {service.Id}");
        }   

        //var team = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Team");

        var referralDto = CreateReferralDto(model, ProfessionalUser, /*team,*/ service, organisation);

        return await _referralClientService.CreateReferral(referralDto);
    }

    private async Task SendVcsNotificationEmail(
        string professionalEmail,
        string requestNumber,
        string serviceName)
    {
        string? requestsSent = _configuration["RequestsSentUrl"];

        //todo: config exception
        if (string.IsNullOrEmpty(requestsSent))
        {
            throw new InvalidOperationException("RequestsSentUrl not set in config");
        }

        var viewConnectionRequestUrl = new UriBuilder(requestsSent!)
        {
            Path = "VcsRequestForSupport/pagename",
            Query = $"referralId={requestNumber}"
        }.Uri;

        var emailTokens = new Dictionary<string, string>
        {
            { "RequestNumber", requestNumber },
            { "ServiceName", serviceName },
            { "ViewConnectionRequestUrl", viewConnectionRequestUrl.ToString()}
        };

        string? vcsNewRequestTemplateId = _configuration["NotificationTemplateIds:VcsNewRequest"];
        if (string.IsNullOrEmpty(vcsNewRequestTemplateId))
        {
            throw new InvalidOperationException("NotificationTemplateIds:VcsNewRequest not set in config");
        }

        await _notifications.SendEmailAsync(professionalEmail, vcsNewRequestTemplateId, emailTokens);
    }

    private static ReferralDto CreateReferralDto(
        ConnectionRequestModel model,
        FamilyHubsUser user,
        //Claim? team,
        ServiceDto service,
        OrganisationDto organisation)
    {
        var referralDto = new ReferralDto
        {
            ReasonForSupport = model.Reason!,
            EngageWithFamily = model.EngageReason!,
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
            ReferrerDto = new ReferrerDto
            {
                EmailAddress = user.Email,
                Name = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                //Team = team?.Value
            },
            ReferralServiceDto = new ReferralServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                ReferralOrganisationDto = new ReferralOrganisationDto
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
}