using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ContactDetailsModel : ProfessionalReferralCacheModel
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

        //todo: move this and code from CheckDetails into one place

        // handle this edge case:
        // user reaches check details page, then clicks to change Contact methods, then selects a new contact method,
        // then clicks continue, then back to Contact methods page, then back to Check details page.
        // without this, they will have a contact method selected, but without the appropriate contact details.
        // with this, they won't have a back button and will be forced to re-enter contact details.
        if ((ContactMethods[(int) ConnectContactDetailsJourneyPage.Telephone] && model.TelephoneNumber == null)
            || (ContactMethods[(int) ConnectContactDetailsJourneyPage.Textphone] && model.TextphoneNumber == null)
            || (ContactMethods[(int) ConnectContactDetailsJourneyPage.Email] && model.EmailAddress == null)
            || (ContactMethods[(int) ConnectContactDetailsJourneyPage.Letter] &&
                (model.AddressLine1 == null || model.TownOrCity == null || model.Postcode == null)))
        {
            BackUrl = null;
        }
    }

    protected override Task<IActionResult> OnPostWithModelNew(ConnectionRequestModel model)
    {
        if (!(ModelState.IsValid && ContactMethods.Any(m => m)))
        {
            //ValidationValid = false;
            //todo: pass params or object (source array of error enums) to RedirectToProfessionalReferralPage for additional params
            //todo: special handling for redirect to self for p/r/g
            // set ValidationValid directly, or use a generic error csv, which we can convert to an array of errors

            return Task.FromResult(RedirectToSelf(ProfessionalReferralError.ContactDetails_NoContactMethodsSelected));
        }

        model.ContactMethodsSelected = ContactMethods;

        return Task.FromResult(NextPage(FirstContactMethodPage(model.ContactMethodsSelected)));
    }
}
