using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ConsentModel : ProfessionalReferralModel
{ 
    [BindProperty]
    public string? Consent { get; set; }

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public ConsentModel() : base(ConnectJourneyPage.Consent)
    {
    }

    protected override Task<IActionResult> OnSafeGetAsync()
    {
        if (Errors != null)
        {
            ValidationValid = false;
        }
        return Task.FromResult((IActionResult)Page());
    }

    protected override Task<IActionResult> OnSafePostAsync()
    {
        return Task.FromResult(OnSafePost());
    }

    private IActionResult OnSafePost()
    {
        if (!ModelState.IsValid || Consent == null)
        {
            return RedirectToSelf(ProfessionalReferralError.Consent_NoConsentSelected);
        }

        if (string.Compare(Consent, "yes", StringComparison.OrdinalIgnoreCase) == 0)
        {
            return NextPage();
        }

        return RedirectToProfessionalReferralPage("ConsentShutter");
    }
}
