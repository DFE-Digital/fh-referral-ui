using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class ConsentModel : PageModel
{
    public string ReferralId { get; set; } = default!;

    [BindProperty]
    public string IsConsentGiven { get; set; } = default!;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public void OnGet(string id, string name, string referralId)
    {
        Id = id;
        Name = name;
        ReferralId = referralId;
    }

    public IActionResult OnPost(string id, string name, string referralId)
    {
        if (!ModelState.IsValid || IsConsentGiven == null)
        {
            Id = id;
            Name = name;
            ReferralId = referralId;
            ValidationValid = false;
            return Page();
        }

        if (string.Compare(IsConsentGiven, "yes", StringComparison.OrdinalIgnoreCase) == 0)
        {
            return RedirectToPage("/ProfessionalReferral/FamilyContact", new
            {
                id = id,
                name = name,
                referralId = referralId
            });
        }

        return RedirectToPage("/ProfessionalReferral/ConsentShutter", new
        {
        });

    }
    
}
