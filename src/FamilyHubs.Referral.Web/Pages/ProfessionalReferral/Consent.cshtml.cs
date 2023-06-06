using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ConsentModel : ProfessionalReferralCacheModel
{ 
    [BindProperty]
    public string? Consent { get; set; }

    public ConsentModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
        : base(ConnectJourneyPage.Consent, connectionRequestDistributedCache)
    {
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid || Consent == null)
        {
            return RedirectToSelf(null, ProfessionalReferralError.Consent_NoConsentSelected);
        }

        if (string.Compare(Consent, "yes", StringComparison.OrdinalIgnoreCase) == 0)
        {
            return NextPage();
        }

        return RedirectToProfessionalReferralPage("ConsentShutter");
    }
}
