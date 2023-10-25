using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Options;
using Microsoft.Extensions.Options;

namespace FamilyHubs.Referral.Web.Pages.My_Account;

public class IndexModel : HeaderPageModel
{
    public string? FullName { get; set; }
    public Uri? GovOneLoginAccountPage { get; set; }

    public IndexModel(IOptions<FamilyHubsUiOptions> familyHubsUiOptions)
    {
        GovOneLoginAccountPage = familyHubsUiOptions.Value.Url(UrlKeys.GovUkLoginAccountPage);
    }

    public void OnGet()
    {
        FullName = HttpContext.GetFamilyHubsUser().FullName;
    }
}