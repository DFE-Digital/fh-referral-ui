using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.SharedKernel.Identity;

namespace FamilyHubs.Referral.Web.Pages.My_Account;

public class IndexModel : HeaderPageModel
{
    public string? FullName { get; set; }
    public string? GovOneLoginAccountPage { get; set; }

    public IndexModel(IConfiguration configuration)
    {
        GovOneLoginAccountPage = configuration.GetValue<string>("GovUkLoginAccountPage")!;
    }

    public void OnGet()
    {
        FullName = HttpContext.GetFamilyHubsUser().FullName;
    }
}