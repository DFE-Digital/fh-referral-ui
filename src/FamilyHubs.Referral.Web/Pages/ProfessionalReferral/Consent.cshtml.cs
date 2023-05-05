using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ConsentModel : ProfessionalReferralModel
{ 
    [BindProperty]
    public string? Consent { get; set; }

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public IActionResult OnPost(string serviceId, string serviceName)
    {
        if (!ModelState.IsValid || Consent == null)
        {
            ValidationValid = false;
            return Page();
        }

        if (string.Compare(Consent, "yes", StringComparison.OrdinalIgnoreCase) == 0)
        {
            return RedirectToPage("/ProfessionalReferral/SupportDetails", new
            {
                serviceId,
                serviceName
            });
        }

        return RedirectToPage("/ProfessionalReferral/ConsentShutter");
    }
}
