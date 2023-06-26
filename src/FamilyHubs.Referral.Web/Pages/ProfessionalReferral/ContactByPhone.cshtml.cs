using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ContactByPhoneModel : ProfessionalReferralCacheModel
{
    public enum ContactType
    {
        Email,
        TelephoneAndEmail
    }

    //[Required(ErrorMessage = "Enter a UK telephone number", AllowEmptyStrings = false)]
    //[UkGdsTelephoneNumber]
    [BindProperty]
    public string? TelephoneNumber { get; set; }

    [BindProperty]
    public ContactType? Contact { get; set; }

    public ContactByPhoneModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
        : base(ConnectJourneyPage.ContactByPhone, connectionRequestDistributedCache)
    {
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToSelf(null, ProfessionalReferralError.Consent_NoConsentSelected);
        }

        return NextPage();
    }
}