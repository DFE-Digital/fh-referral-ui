
namespace FamilyHubs.Referral.Core.Notifications;

public interface INotifications
{
    Task SendEmailAsync(string emailAddress, string templateId, IDictionary<string, string> tokens);
}