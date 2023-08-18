using FamilyHubs.Notification.Api.Client;
using FamilyHubs.Notification.Api.Client.Templates;
using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
    private readonly FamilyHubsUiOptions _familyHubsUiOptions;
    private readonly ILogger<ReferralNotificationService> _logger;

    public ReferralNotificationService(
        INotifications notifications,
        INotificationTemplates<NotificationType> notificationTemplates,
        IIdamsClient idamsClient,
        IOptions<FamilyHubsUiOptions> familyHubsUiOptions,
        ILogger<ReferralNotificationService> logger)
    {
        _notifications = notifications;
        _notificationTemplates = notificationTemplates;
        _idamsClient = idamsClient;
        _familyHubsUiOptions = familyHubsUiOptions.Value;
        _logger = logger;
    }

    public async Task OnCreateReferral(
        string laProfessionalEmailAddress,
        long serviceOrgansiationId,
        string serviceName,
        int requestNumber)
    {
        await TrySendVcsNotificationEmails(serviceOrgansiationId, serviceName, requestNumber);

        await TrySendProfessionalNotificationEmails(laProfessionalEmailAddress, serviceName, requestNumber);
    }

    private async Task TrySendVcsNotificationEmails(
        long organisationId,
        string serviceName,
        int requestNumber)
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
            await SendNotificationEmails(emailAddresses, NotificationType.VcsNewRequest, requestNumber, serviceName);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Unable to send VcsNewRequest email(s) for request {RequestNumber}", requestNumber);
        }
    }

    private async Task TrySendProfessionalNotificationEmails(
        string emailAddress,
        string serviceName,
        int requestNumber)
    {
        try
        {
            await SendNotificationEmails(new List<string> { emailAddress },
                NotificationType.ProfessionalSentRequest, requestNumber, serviceName);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Unable to send ProfessionalSentRequest email for request {RequestNumber}", requestNumber);
        }
    }

    private async Task SendNotificationEmails(
        IEnumerable<string> vcsEmailAddresses,
        NotificationType notificationType,
        int requestNumber,
        string serviceName)
    {
        string path = notificationType == NotificationType.ProfessionalSentRequest
            ? "La"
            : "Vcs";

        var viewConnectionRequestUrl =
            _familyHubsUiOptions.Url(UrlKeys.DashboardWeb, $"{path}/RequestDetails?id={requestNumber}").ToString();

        var emailTokens = new Dictionary<string, string>
        {
            { "RequestNumber", requestNumber.ToString("X6") },
            { "ServiceName", serviceName },
            { "ViewConnectionRequestUrl", viewConnectionRequestUrl}
        };

        string templateId = _notificationTemplates.GetTemplateId(notificationType);

        await _notifications.SendEmailsAsync(vcsEmailAddresses, templateId, emailTokens);
    }
}