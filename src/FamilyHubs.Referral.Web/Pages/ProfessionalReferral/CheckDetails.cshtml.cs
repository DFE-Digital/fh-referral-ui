using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Extensions;
using FamilyHubs.SharedKernel.Identity.Models;
using Microsoft.AspNetCore.Mvc;
using FamilyHubs.Referral.Core.Notifications;
using SharedOrganisationDto = FamilyHubs.ServiceDirectory.Shared.Dto.OrganisationDto;
using ReferralOrganisationDto = FamilyHubs.ReferralService.Shared.Dto.OrganisationDto;

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

        int requestNumber = await CreateConnectionRequest(service, model);

        string dashboardUrl = GetDashboardUrl();

        //todo: will need to get vcs org's emails from idams and pass through
        await TrySendVcsNotificationEmails(
            Enumerable.Empty<string>(), service.Name, requestNumber, dashboardUrl);

        await TrySendProfessionalNotificationEmails(
            ProfessionalUser.Email, service.Name, requestNumber, dashboardUrl);

        //todo: need to send the non-hex version
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

        string referralIdBase10 = await _referralClientService.CreateReferral(referralDto);

        return int.Parse(referralIdBase10);
    }

    private async Task TrySendVcsNotificationEmails(
        IEnumerable<string> emailAddresses,
        string serviceName,
        int requestNumber,
        string dashboardUrl)
    {
        if (!emailAddresses.Any())
        {
            _logger.LogWarning("VCS organisation has no email addresses. Unable to send VcsNewRequest email for request {RequestNumber}", requestNumber);
            return;
        }

        try
        {
            //todo: add callback to API, so that we can flag invalid emails/unsent emails
            //todo: as we silently chomp any exceptions, should we just fire and forget?
            await SendVcsNotificationEmails(emailAddresses, requestNumber, serviceName, dashboardUrl);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Unable to send VcsNewRequest email(s) for request {RequestNumber}", requestNumber);
            throw;
        }
    }

    private async Task TrySendProfessionalNotificationEmails(
        string emailAddress, string serviceName, int requestNumber, string dashboardUrl)
    {
        try
        {
            await SendProfessionalNotificationEmails(emailAddress, serviceName, requestNumber, dashboardUrl);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Unable to send ProfessionalSentRequest email for request {RequestNumber}", requestNumber);
            throw;
        }
    }

    private string GetDashboardUrl()
    {
        string? requestsSent = _configuration["RequestsSentUrl"];

        //todo: config exception
        if (string.IsNullOrEmpty(requestsSent))
        {
            //todo: use config exception
            throw new InvalidOperationException("RequestsSentUrl not set in config");
        }

        return requestsSent;
    }

    private async Task SendProfessionalNotificationEmails(
        string emailAddress, string serviceName, int requestNumber, string dashboardUrl)
    {
        string? professionalSentRequestTemplateId = _configuration["Notification:TemplateIds:ProfessionalSentRequest"];
        if (string.IsNullOrEmpty(professionalSentRequestTemplateId))
        {
            //todo: use config exception
            throw new InvalidOperationException("Notification:TemplateIds:ProfessionalSentRequest not set in config");
        }

        var viewConnectionRequestUrl = new UriBuilder(dashboardUrl)
        {
            Path = "La/RequestDetails",
            Query = $"referralId={requestNumber}"
        }.Uri;

        var emailTokens = new Dictionary<string, string>
        {
            { "RequestNumber", requestNumber.ToString("X6") },
            { "ServiceName", serviceName },
            { "ViewConnectionRequestUrl", viewConnectionRequestUrl.ToString()}
        };

        await _notifications.SendEmailsAsync(
            new List<string> { emailAddress }, professionalSentRequestTemplateId, emailTokens);
    }

    private async Task SendVcsNotificationEmails(
        IEnumerable<string> vcsEmailAddresses,
        int requestNumber,
        string serviceName,
        string dashboardUrl)
    {
        var viewConnectionRequestUrl = new UriBuilder(dashboardUrl)
        {
            Path = "Vcs/RequestDetails",
            Query = $"referralId={requestNumber}"
        }.Uri;

        var emailTokens = new Dictionary<string, string>
        {
            { "RequestNumber", requestNumber.ToString("X6") },
            { "ServiceName", serviceName },
            { "ViewConnectionRequestUrl", viewConnectionRequestUrl.ToString()}
        };

        string? vcsNewRequestTemplateId = _configuration["Notification:TemplateIds:VcsNewRequest"];
        if (string.IsNullOrEmpty(vcsNewRequestTemplateId))
        {
            //todo: use config exception
            throw new InvalidOperationException("Notification:TemplateIds:VcsNewRequest not set in config");
        }

        await _notifications.SendEmailsAsync(vcsEmailAddresses, vcsNewRequestTemplateId, emailTokens);
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