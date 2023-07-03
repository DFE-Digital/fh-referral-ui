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
using FamilyHubs.ServiceDirectory.Shared.Enums;
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

        string requestNumber = await CreateConnectionRequest(service, model);

        await TrySendVcsNotificationEmails(service, requestNumber);

        return RedirectToPage("/ProfessionalReferral/Confirmation", new { requestNumber });
    }

    private async Task<OrganisationDto> GetOrganisation(ServiceDto service)
    {
        OrganisationDto? organisation = await _organisationClientService.GetOrganisationDtobyIdAsync(service.OrganisationId);

        if (organisation == null)
        {
            //todo: create and throw custom exception
            throw new InvalidOperationException($"Organisation not found for service {service.Id}");
        }
        return organisation;
    }

    private async Task<string> CreateConnectionRequest(
        ServiceDto service,
        ConnectionRequestModel model)
    {
        var organisation = await GetOrganisation(service);

        //todo: should we check if the organisation is a VCFS organisation?
        //organisation.OrganisationType == OrganisationType.VCFS

        //var team = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Team");

        var referralDto = CreateReferralDto(model, ProfessionalUser, /*team,*/ service, organisation);

        string referralIdBase10 = await _referralClientService.CreateReferral(referralDto);

        //todo: do this in API?
        return int.Parse(referralIdBase10).ToString("X6");
    }

    private async Task TrySendVcsNotificationEmails(ServiceDto service, string requestNumber)
    {
        var serviceEmail = service.Contacts
            .Where(c => !string.IsNullOrEmpty(c.Email))
            .Select(c => c.Email!);
        if (!serviceEmail.Any())
        {
            _logger.LogWarning("Service {ServiceId} has no email addresses. Unable to send VcsNewRequest email for request {RequestNumber}", service.Id, requestNumber);
        }
        else
        {
            try
            {
                //todo: would be better if the API accepted multiple emails
                //todo: add callback to API, so that we can flag invalid emails/unsent emails
                var sendEmailTasks = serviceEmail.Select(email =>
                    SendVcsNotificationEmail(email, requestNumber, service.Name));
                await Task.WhenAll(sendEmailTasks);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Unable to send VcsNewRequest email(s) for request {RequestNumber}", requestNumber);
                throw;
            }
        }
    }

    private async Task SendVcsNotificationEmail(
        string vcsEmailAddress,
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
            Path = "VcsRequestForSupport/Request",
            Query = $"referralId={requestNumber}"
        }.Uri;

        var emailTokens = new Dictionary<string, string>
        {
            { "RequestNumber", requestNumber },
            { "ServiceName", serviceName },
            { "ViewConnectionRequestUrl", viewConnectionRequestUrl.ToString()}
        };

        string? vcsNewRequestTemplateId = _configuration["Notification:TemplateIds:VcsNewRequest"];
        if (string.IsNullOrEmpty(vcsNewRequestTemplateId))
        {
            throw new InvalidOperationException("Notification:TemplateIds:VcsNewRequest not set in config");
        }

        await _notifications.SendEmailAsync(vcsEmailAddress, vcsNewRequestTemplateId, emailTokens);
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