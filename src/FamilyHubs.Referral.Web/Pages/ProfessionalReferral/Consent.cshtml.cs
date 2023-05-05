using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ConsentModel : ProfessionalReferralNoSessionModel
{ 
    [BindProperty]
    public string? Consent { get; set; }

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    protected override Task<IActionResult> OnSafePostAsync(string serviceId, string serviceName)
    {
        return Task.FromResult(OnSafePost(serviceId, serviceName));
    }

    protected IActionResult OnSafePost(string serviceId, string serviceName)
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
