
namespace FamilyHubs.Referral.Core.Models;

public enum ContactMethod
{
    Email,
    Telephone,
    Textphone,
    Letter,
    Last = Letter
}

public class ConnectionRequestModel
{
    public string? ServiceId { get; set; }
    public string? ServiceName { get; set; }
    public string? FamilyContactFullName { get; set; }
    public string? Reason { get; set; }
    public bool[] ContactMethodsSelected { get; set; } = new bool[(int)ContactMethod.Last+1];
    public string? EmailAddress { get; set; }
    public string? TelephoneNumber { get; set; }
    public string? TextphoneNumber { get; set; }
}