using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ContactDetailsModel : ProfessionalReferralSessionModel
{
    public string? FullName { get; set; }

    [BindProperty]
    public bool[] ContactMethods { get; set; } = new bool[(int)ConnectJourneyPage.LastContactMethod+1];

    public ContactDetailsModel(IConnectionRequestDistributedCache connectionRequestCache) : base(connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        FullName = model.FamilyContactFullName;
        ContactMethods = model.ContactMethodsSelected;
    }

    protected override string? OnPostWithModel(ConnectionRequestModel model)
    {
        if (!(ModelState.IsValid && ContactMethods.Any(m => m)))
        {
            ValidationValid = false;
            return null;
        }

        model.ContactMethodsSelected = ContactMethods;

        // if the user has come from the check details page,
        //if (Flow == JourneyFlow.ChangingContactMethods)
        //{
        // we need to remove any previous contact details that are no longer selected
        //todo: can we do this generically?
        if (!ContactMethods[(int) ConnectJourneyPage.Email])
        {
            model.EmailAddress = null;
        }
        if (!ContactMethods[(int)ConnectJourneyPage.Telephone])
        {
            model.TelephoneNumber = null;
        }
        if (!ContactMethods[(int)ConnectJourneyPage.Textphone])
        {
            model.TextphoneNumber = null;
        }
        if (!ContactMethods[(int)ConnectJourneyPage.Letter])
        {
            model.AddressLine1 = null;
            model.AddressLine2 = null;
            model.TownOrCity = null;
            model.County = null;
            model.Postcode = null;
        }
        //}
        return FirstContactMethodPage(model.ContactMethodsSelected);
    }
}
