using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class PersonRequestingSupportModel : PageModel
{
    [BindProperty]
    public string IsTypeOfPerson { get; set; } = default!;
    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid || IsTypeOfPerson == null)
        {
            ValidationValid = false;
            return Page();
        }
            
        if (string.Compare(IsTypeOfPerson, "professional", StringComparison.OrdinalIgnoreCase) == 0)
        {
            return RedirectToPage("/ProfessionalReferral/SignIn", new
            {

            });
        }
        else if (string.Compare(IsTypeOfPerson, "vcsadmin", StringComparison.OrdinalIgnoreCase) == 0)
        {
            return RedirectToPage("/Vcs/SignIn", new
            {

            });
        }

        return RedirectToPage("/ProfessionalReferral/Search", new
        {
        });
    }
}
