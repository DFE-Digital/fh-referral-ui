using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class SignInModel : PageModel
{
    [BindProperty]
    public string Id { get; set; } = default!;

    [BindProperty]
    public string Name { get; set; } = default!;

    [BindProperty]
    public string Email { get; set; } = string.Empty;
    [BindProperty]
    public string Password { get; set; } = string.Empty;
    public void OnGet(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public IActionResult OnPost()
    {
        return RedirectToPage("/ProfessionalReferral/Safeguarding", new
        {
            id = Id,
            name = Name
        });

    }
}
