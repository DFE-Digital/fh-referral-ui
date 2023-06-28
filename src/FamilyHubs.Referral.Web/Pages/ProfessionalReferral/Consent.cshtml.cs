using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ConsentModel : ProfessionalReferralCacheModel
{ 
    [BindProperty]
    public bool? Consent { get; set; }

    public ConsentModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
        : base(ConnectJourneyPage.Consent, connectionRequestDistributedCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        if (model.ConsentGiven)
        {
            Consent = true;
        }
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid || Consent == null)
        {
            return RedirectToSelf(null, ErrorId.Consent_NoConsentSelected);
        }

        if (Consent.Value)
        {
            model.ConsentGiven = true;
            return NextPage();
        }

        return RedirectToProfessionalReferralPage("ConsentShutter");
    }
}
