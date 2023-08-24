using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Delegators;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.Shared;

public class HeaderPageModel : PageModel, IFamilyHubsHeader
{
    public bool ShowActionLinks => User.Identity?.IsAuthenticated == true;
    public bool ShowNavigationMenu => true;

    LinkStatus IFamilyHubsHeader.GetStatus(FhLinkOptions link)
    {
        return link.Text == "Search for service" ? LinkStatus.Active : LinkStatus.Visible;
    }
}