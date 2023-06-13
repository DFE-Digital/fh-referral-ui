
namespace FamilyHubs.Referral.Core.Models;

public class ConnectionRequestModel
{
    public string? ServiceId { get; set; }
    public string? FamilyContactFullName { get; set; }
    public string? Reason { get; set; }
    public bool[] ContactMethodsSelected { get; set; } = new bool[(int)ConnectContactDetailsJourneyPage.LastContactMethod+1];
    public string? EmailAddress { get; set; }
    public string? TelephoneNumber { get; set; }
    public string? TextphoneNumber { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? TownOrCity { get; set; }
    public string? County { get; set; }
    public string? Postcode { get; set; }
    public string? EngageReason { get; set; }

    public ProfessionalReferralErrorState? ErrorState { get; set; }

    public void RemoveNonSelectedContactDetails()
    {
        //todo: can we do this generically?
        if (!ContactMethodsSelected[(int)ConnectContactDetailsJourneyPage.Email])
        {
            EmailAddress = null;
        }

        if (!ContactMethodsSelected[(int)ConnectContactDetailsJourneyPage.Telephone])
        {
            TelephoneNumber = null;
        }

        if (!ContactMethodsSelected[(int)ConnectContactDetailsJourneyPage.Textphone])
        {
            TextphoneNumber = null;
        }

        if (!ContactMethodsSelected[(int)ConnectContactDetailsJourneyPage.Letter])
        {
            AddressLine1 = null;
            AddressLine2 = null;
            TownOrCity = null;
            County = null;
            Postcode = null;
        }
    }
}