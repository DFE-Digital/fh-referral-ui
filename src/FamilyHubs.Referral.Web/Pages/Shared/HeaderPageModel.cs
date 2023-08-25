﻿using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;
using FamilyHubs.SharedKernel.Razor.Header;
using FamilyHubs.SharedKernel.Razor.Links;
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
    public bool ShowNavigationMenu => User.Identity?.IsAuthenticated == true;

    LinkStatus IFamilyHubsHeader.GetStatus(IFhRenderLink link)
    {
        return _highlightSearchForService
        && link.Text == "Search for service" ? LinkStatus.Active : LinkStatus.Visible;
    }

    IEnumerable<IFhRenderLink> IFamilyHubsHeader.NavigationLinks(
        FhLinkOptions[] navigationLinks,
        IFamilyHubsUiOptions familyHubsUiOptions)
    {
        string role = HttpContext.GetRole();
        return role is RoleTypes.VcsProfessional or RoleTypes.VcsDualRole
               ? navigationLinks.Reverse()
               : navigationLinks;
    }
}