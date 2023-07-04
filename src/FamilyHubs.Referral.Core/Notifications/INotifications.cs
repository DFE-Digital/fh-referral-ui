
namespace FamilyHubs.Referral.Core.Notifications;

public interface INotifications
{
    Task SendEmailsAsync(
        IEnumerable<string> emailAddresses,
        string templateId,
        IDictionary<string, string> tokens,
        CancellationToken cancellationToken = default);
}