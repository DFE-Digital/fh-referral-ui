using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Delegators;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.Shared;

public class HeaderPageModel : PageModel, IFamilyHubsHeader
{
    private readonly bool _highlightSearchForService;

    public HeaderPageModel(bool highlightSearchForService = true)
    {
        _highlightSearchForService = highlightSearchForService;
    }

    public bool ShowActionLinks => User.Identity?.IsAuthenticated == true;
    public bool ShowNavigationMenu => true;

    LinkStatus IFamilyHubsHeader.GetStatus(FhLinkOptions link)
    {
        return _highlightSearchForService
        && link.Text == "Search for service" ? LinkStatus.Active : LinkStatus.Visible;
    }

    IEnumerable<FhLinkOptions> IFamilyHubsHeader.NavigationLinks(FhLinkOptions[] navigationLinks)
    {
        string role = HttpContext.GetRole();
        return role is RoleTypes.VcsProfessional or RoleTypes.VcsDualRole
               ? navigationLinks.Reverse()
               : navigationLinks;
    }
}