using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ConsentModel : ProfessionalReferralModel
{ 
    [BindProperty]
    public string? Consent { get; set; }

    public ConsentModel() : base(ConnectJourneyPage.Consent)
    {
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
