using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class ConsentModel : PageModel
{
    [BindProperty]
    public string IsConsentGiven { get; set; } = default!;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public void OnGet(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public IActionResult OnPost(string id, string name)
    {
        if (!ModelState.IsValid || IsConsentGiven == null)
        {
            ValidationValid = false;
            return Page();
        }

        if (string.Compare(IsConsentGiven, "yes", StringComparison.OrdinalIgnoreCase) == 0)
        {
            return RedirectToPage("/ProfessionalReferral/PersonalDetails", new
            {
                id = id,
                name = name
            });
        }

        return RedirectToPage("/ProfessionalReferral/ConsentShutter", new
        {
        });

    }
    
}
