using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.SharedKernel.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.sign_out;

public class IndexModel : PageModel
{
    private readonly IConnectionRequestDistributedCache _connectionRequestCache;

    public IndexModel(IConnectionRequestDistributedCache connectionRequestCache)
    {
        _connectionRequestCache = connectionRequestCache;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var professionalUser = HttpContext.GetFamilyHubsUser();
        await _connectionRequestCache.RemoveAsync(professionalUser.Email);
        return Redirect("/account/signout");
    }
}