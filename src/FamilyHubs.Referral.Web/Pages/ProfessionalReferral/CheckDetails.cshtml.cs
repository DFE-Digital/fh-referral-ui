using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class CheckDetailsModel : ProfessionalReferralSessionModel
{
    public ConnectionRequestModel? ConnectionRequestModel { get; set; }

    public CheckDetailsModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(ConnectJourneyPage.CheckDetails, connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        // do this now, so we don't display any previously entered contact details that are no longer selected
        // but don't remove them from the session yet, in case the user goes back to change the contact details
        // we'll remove them from the session when the user submits the form
        RemoveNonSelectedContactDetails(model);

        ConnectionRequestModel = model;
    }

    private static void RemoveNonSelectedContactDetails(ConnectionRequestModel model)
    {
        // remove any previously entered contact details that are no longer selected
        //todo: can we do this generically?
        if (!model.ContactMethodsSelected[(int) ConnectContactDetailsJourneyPage.Email])
        {
            model.EmailAddress = null;
        }

        if (!model.ContactMethodsSelected[(int) ConnectContactDetailsJourneyPage.Telephone])
        {
            model.TelephoneNumber = null;
        }

        if (!model.ContactMethodsSelected[(int) ConnectContactDetailsJourneyPage.Textphone])
        {
            model.TextphoneNumber = null;
        }

        if (!model.ContactMethodsSelected[(int) ConnectContactDetailsJourneyPage.Letter])
        {
            model.AddressLine1 = null;
            model.AddressLine2 = null;
            model.TownOrCity = null;
            model.County = null;
            model.Postcode = null;
        }
    }

    protected override string? OnPostWithModel(ConnectionRequestModel model)
    {
        RemoveNonSelectedContactDetails(model);

        return "Confirmation";
    }
}