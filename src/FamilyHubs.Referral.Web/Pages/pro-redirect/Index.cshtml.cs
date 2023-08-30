using FamilyHubs.Referral.Core.Models;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace FamilyHubs.Referral.Web.Pages.pro_redirect;

[Authorize]
public class IndexModel : PageModel
{
    private readonly FamilyHubsUiOptions _familyHubsUiOptions;

    public IndexModel(IOptions<FamilyHubsUiOptions> familyHubsUiOptions)
    {
        _familyHubsUiOptions = familyHubsUiOptions.Value;
    }

    public IActionResult OnGet()
    {
        var user = HttpContext.GetFamilyHubsUser();

        string redirect = user.Role switch
        {
            // this case should be picked up by the middleware, but we leave it here, so that there's no way we can end up with a 403, when it should be a 401
            null or "" => "/Error/401",
            RoleTypes.VcsProfessional or RoleTypes.VcsDualRole => _familyHubsUiOptions.Url(UrlKeys.DashboardWeb, "vcs/dashboard").ToString(),
            RoleTypes.LaProfessional or RoleTypes.LaDualRole => "/ProfessionalReferral/Search",
            _ => "/Error/403"
        };

        return Redirect(redirect);
    }
}