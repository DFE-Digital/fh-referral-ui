using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ServiceDirectory.Ui.Pages.Vcs;

public class SignInModel : PageModel
{
    [BindProperty]
    public string Email { get; set; } = string.Empty;
    [BindProperty]
    public string Password { get; set; } = string.Empty;
    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        return RedirectToPage("/Vcs/ShowReferralSummary", new
        {
        });
    }
}
