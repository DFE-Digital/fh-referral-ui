using FamilyHubs.SharedKernel.Razor.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.cookies;

public class IndexModel : PageModel
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