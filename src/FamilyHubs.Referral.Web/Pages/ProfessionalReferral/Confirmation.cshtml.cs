using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Notifications;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Delegators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

[Authorize]
public class ConfirmationModel : PageModel, IFamilyHubsHeader
{
    private readonly IConnectionRequestDistributedCache _connectionRequestCache;
    private readonly INotifications _notifications;

    public ConfirmationModel(
        IConnectionRequestDistributedCache connectionRequestCache,
        INotifications notifications)
    {
        _connectionRequestCache = connectionRequestCache;
        _notifications = notifications;
    }

    public int RequestNumber { get; set; }

    public async Task OnGetAsync(int requestNumber)
    {
        var professionalUser = HttpContext.GetFamilyHubsUser();
        await _connectionRequestCache.RemoveAsync(professionalUser.Email);

        RequestNumber = requestNumber;

        string requestNumberString = requestNumber.ToString();

        var emailTokens = new Dictionary<string, string>
        {
            { "RequestNumber", requestNumberString },
            //todo: change base now?, or move to details post?
            //{ "ServiceName", service.Name }
            //todo: page & from config
            //{"ViewConnectionRequestUrl", $"https://test.manage-connection-requests.education.gov.uk/VcsRequestForSupport/pagename?referralId={requestNumberString}"}
        };

        //todo: from config
        await _notifications.SendEmailAsync(professionalUser.Email, "d460f57c-9c5e-4c33-8420-cdde4fca85c2", emailTokens);
    }

    LinkStatus IFamilyHubsHeader.GetStatus(SharedKernel.Razor.FamilyHubsUi.Options.LinkOptions link)
    {
        return link.Text == "Search for service" ? LinkStatus.Active : LinkStatus.Visible;
    }
}