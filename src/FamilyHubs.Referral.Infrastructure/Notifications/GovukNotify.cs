using FamilyHubs.Referral.Core.Notifications;

namespace FamilyHubs.Referral.Infrastructure.Notifications;

public class GovukNotify : INotifications
{
    public Task SendEmailAsync(string emailAddress, string templateId, IDictionary<string, string> tokens)
    {
        throw new NotImplementedException();
    }
}