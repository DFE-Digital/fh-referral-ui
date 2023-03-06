using Microsoft.Extensions.Options;
using Notify.Interfaces;

namespace FamilyHubs.ReferralUi.Ui.Services;

public class GovNotifySetting
{
    public string APIKey { get; set; } = default!;
    public string TemplateId { get; set; } = default!;
}

public interface IEmailSender
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}

public class GovNotifySender : IEmailSender
{
    private readonly IOptions<GovNotifySetting> _govNotifySettings;
    private readonly IAsyncNotificationClient _notificationClient;

    public GovNotifySender(IOptions<GovNotifySetting> govNotifySettings, IAsyncNotificationClient notificationClient)
    {
        _govNotifySettings = govNotifySettings;
        _notificationClient = notificationClient;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Dictionary<String, dynamic> personalisation = new Dictionary<string, dynamic>
        {
            {"subject", subject},
            {"htmlMessage", htmlMessage}
        };

        await _notificationClient.SendEmailAsync(
                emailAddress: email,
                templateId: _govNotifySettings.Value.TemplateId,
                personalisation: personalisation,
                clientReference: null,
                emailReplyToId: null
        );
    }
}
