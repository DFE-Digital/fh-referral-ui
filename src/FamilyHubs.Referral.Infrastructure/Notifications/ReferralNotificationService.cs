using FamilyHubs.Notification.Api.Client;
using FamilyHubs.Notification.Api.Client.Templates;
using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Core.ApiClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.Referral.Infrastructure.Notifications;

public enum NotificationType
{
    ProfessionalSentRequest,
    VcsNewRequest
}

public class ReferralNotificationService : IReferralNotificationService
{
    private readonly INotifications _notifications;
    private readonly INotificationTemplates<NotificationType> _notificationTemplates;
    private readonly IIdamsClient _idamsClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReferralNotificationService> _logger;

    public ReferralNotificationService(
        INotifications notifications,
        INotificationTemplates<NotificationType> notificationTemplates,
        IIdamsClient idamsClient,
        IConfiguration configuration,
        ILogger<ReferralNotificationService> logger)
    {
        _notifications = notifications;
        _notificationTemplates = notificationTemplates;
        _idamsClient = idamsClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task OnCreateReferral(
        string laProfessionalEmailAddress,
        long serviceOrgansiationId,
        string serviceName,
        int requestNumber)
    {
        string dashboardUrl = GetDashboardUrl();

        await TrySendVcsNotificationEmails(
            serviceOrgansiationId, serviceName, requestNumber, dashboardUrl);

        await TrySendProfessionalNotificationEmails(
            laProfessionalEmailAddress, serviceName, requestNumber, dashboardUrl);
    }

    private async Task TrySendVcsNotificationEmails(
    long organisationId,
    string serviceName,
    int requestNumber,
    string dashboardUrl)
    {
        var emailAddresses = await _idamsClient.GetVcsProfessionalsEmailsAsync(organisationId);
        if (!emailAddresses.Any())
        {
            _logger.LogWarning("VCS organisation has no email addresses. Unable to send VcsNewRequest email for request {RequestNumber}", requestNumber);
            return;
        }

        try
        {
            //todo: add callback to API, so that we can flag invalid emails/unsent emails
            //todo: as we silently chomp any exceptions, should we just fire and forget?
            await SendNotificationEmails(emailAddresses, NotificationType.VcsNewRequest, requestNumber, serviceName, dashboardUrl);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Unable to send VcsNewRequest email(s) for request {RequestNumber}", requestNumber);
        }
    }

    private async Task TrySendProfessionalNotificationEmails(
        string emailAddress, string serviceName, int requestNumber, string dashboardUrl)
    {
        try
        {
            await SendNotificationEmails(new List<string> { emailAddress },
                NotificationType.ProfessionalSentRequest, requestNumber, serviceName, dashboardUrl);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Unable to send ProfessionalSentRequest email for request {RequestNumber}", requestNumber);
        }
    }

    private string GetDashboardUrl()
    {
        string? requestsSent = _configuration["RequestsSentUrl"];

        if (string.IsNullOrEmpty(requestsSent))
        {
            //todo: use config exception
            throw new InvalidOperationException("RequestsSentUrl not set in config");
        }

        return requestsSent;
    }

    private async Task SendNotificationEmails(
        IEnumerable<string> vcsEmailAddresses,
        NotificationType notificationType,
        int requestNumber,
        string serviceName,
        string dashboardUrl)
    {
        var viewConnectionRequestUrl = new UriBuilder(dashboardUrl)
        {
            Path = $"{notificationType}/RequestDetails",
            Query = $"id={requestNumber}"
        }.Uri;

        var emailTokens = new Dictionary<string, string>
        {
            { "RequestNumber", requestNumber.ToString("X6") },
            { "ServiceName", serviceName },
            { "ViewConnectionRequestUrl", viewConnectionRequestUrl.ToString()}
        };

        string templateId = _notificationTemplates.GetTemplateId(notificationType);

        await _notifications.SendEmailsAsync(vcsEmailAddresses, templateId, emailTokens);
    }
}