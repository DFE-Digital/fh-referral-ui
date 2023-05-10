using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

[Authorize]
public class ConsentModel : ProfessionalReferralModel
{ 
    [BindProperty]
    public string? Consent { get; set; }

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    protected override Task<IActionResult> OnSafePostAsync()
    {
        return Task.FromResult(OnSafePost());
    }

    private IActionResult OnSafePost()
    {
        if (!ModelState.IsValid || Consent == null)
        {
            ValidationValid = false;
            return Page();
        }

        if (string.Compare(Consent, "yes", StringComparison.OrdinalIgnoreCase) == 0)
        {
            return RedirectToProfessionalReferralPage("SupportDetails");
        }

        return RedirectToPage("/ProfessionalReferral/ConsentShutter");
    }
}
