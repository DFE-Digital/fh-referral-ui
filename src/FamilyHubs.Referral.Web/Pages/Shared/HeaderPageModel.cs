using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Identity.Models;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Delegators;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.Shared;

public class HeaderPageModel : PageModel, IFamilyHubsHeader
{
    //too early...
    protected FamilyHubsUser FamilyHubsUser { get; }

    public HeaderPageModel()
    {
        //todo: reuse in derived classes
        FamilyHubsUser = HttpContext.GetFamilyHubsUser();
    }

    public bool ShowActionLinks => User.Identity?.IsAuthenticated == true;
    public bool ShowNavigationMenu => true;

    LinkStatus IFamilyHubsHeader.GetStatus(FhLinkOptions link)
    {
        //todo: no active if home page
        return link.Text == "Search for service" ? LinkStatus.Active : LinkStatus.Visible;
    }

    IEnumerable<FhLinkOptions> IFamilyHubsHeader.NavigationLinks(FhLinkOptions[] navigationLinks)
    {
        string role = HttpContext.GetRole();
        return role is RoleTypes.VcsProfessional or RoleTypes.VcsDualRole
               ? navigationLinks.Reverse()
               : navigationLinks;
    }
}