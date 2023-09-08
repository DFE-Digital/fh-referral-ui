using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.SharedKernel.Razor.Cookies;

namespace FamilyHubs.Referral.Web.Pages.cookies;

public class IndexModel : HeaderPageModel
{
    public readonly ICookiePage CookiePage;

    public IndexModel(ICookiePage cookiePage)
    {
        CookiePage = cookiePage;
    }

    public void OnPost(bool analytics)
    {
        CookiePage.OnPost(analytics, Request, Response);
    }
}