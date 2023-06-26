
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
    public ReferrerContactType? ReferrerContact { get; set; }
    public string? ReferrerTelephone { get; set; }

    public ProfessionalReferralErrorState? ErrorState { get; set; }
}