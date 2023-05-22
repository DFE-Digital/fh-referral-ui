using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

//todo: error enum per error / page / journey??
//could have every error message in config. would we want to do that?
public enum ProfessionalReferralError
{
    ContactDetailsNoContactMethodsSelected
}

public class ContactDetailsModel : ProfessionalReferralSessionModel
{
    public string? FullName { get; set; }

    [BindProperty]
    public bool[] ContactMethods { get; set; } = new bool[(int)ConnectContactDetailsJourneyPage.LastContactMethod+1];

    public ContactDetailsModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(ConnectJourneyPage.ContactDetails, connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        FullName = model.FamilyContactFullName;
        if (Errors == null)
        {
            ContactMethods = model.ContactMethodsSelected;
        }
    }

    protected override Task<IActionResult> OnPostWithModelNew(ConnectionRequestModel model)
    {
        if (!(ModelState.IsValid && ContactMethods.Any(m => m)))
        {
            //ValidationValid = false;
            //todo: pass params or object (source array of error enums) to RedirectToProfessionalReferralPage for additional params
            //todo: special handling for redirect to self for p/r/g
            return Task.FromResult<IActionResult>(RedirectToPage("/ProfessionalReferral/ContactDetails", new
            {
                ServiceId,
                // set ValidationValid directly, or use a generic error csv, which we can convert to an array of errors
                errors = ProfessionalReferralError.ContactDetailsNoContactMethodsSelected
            }));
        }

        model.ContactMethodsSelected = ContactMethods;

        return Task.FromResult(NextPage(FirstContactMethodPage(model.ContactMethodsSelected)));
    }
}
