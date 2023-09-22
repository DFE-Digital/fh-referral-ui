using System.Security.Claims;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;
using FamilyHubs.SharedKernel.Razor.Header;
using FamilyHubs.SharedKernel.Razor.Links;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.Shared;

public class HeaderPageModel : PageModel, IFamilyHubsHeader
{
    private readonly string? _appHost;
    private readonly bool _highlightSearchForService;

    public HeaderPageModel(IConfiguration configuration, bool highlightSearchForService = true)
    {
        _appHost = configuration["GovUkOidcConfiguration:AppHost"];

        _highlightSearchForService = highlightSearchForService;
    }

    public bool ShowActionLinks => IsAuthenticatedAndTermsAccepted;
    public bool ShowNavigationMenu => IsAuthenticatedAndTermsAccepted;

    private bool IsAuthenticatedAndTermsAccepted =>
        User.Identity?.IsAuthenticated == true
        //&& HttpContext.TermsAndConditionsAccepted();
        && HasAcceptedTerms(User);

    public string? GetClaimValue(ClaimsPrincipal user, string key)
    {
        return user.Claims.FirstOrDefault(x => x.Type == key)?.Value;
    }

    public bool HasAcceptedTerms(ClaimsPrincipal user)
    {
        HttpContext.
        return GetClaimValue(user, $"{FamilyHubsClaimTypes.TermsAndConditionsAccepted}-{_appHost}") != null;
    }

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
            ? familyHubsUiOptions.GetAlternative("VcsHeader").Header.NavigationLinks
            : navigationLinks;
    }
}