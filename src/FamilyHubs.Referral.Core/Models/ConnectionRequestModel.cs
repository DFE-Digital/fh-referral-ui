
namespace FamilyHubs.Referral.Core.Models;

public enum ConnectJourneyPage
{
    Email,
    Telephone,
    Textphone,
    Letter,
    LastContactMethod = Letter,
    ContactMethods
}

public class ConnectionRequestModel
{
    public long? ReferralId { get; set; }
    public string? ServiceId { get; set; }
    public string? FamilyContactFullName { get; set; }
    public string? Reason { get; set; }
    public bool[] ContactMethodsSelected { get; set; } = new bool[(int)ConnectJourneyPage.LastContactMethod+1];
    public string? EmailAddress { get; set; }
    public string? TelephoneNumber { get; set; }
    public string? TextphoneNumber { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? TownOrCity { get; set; }
    public string? County { get; set; }
    public string? Postcode { get; set; }
    public string? EngageReason { get; set; }
}