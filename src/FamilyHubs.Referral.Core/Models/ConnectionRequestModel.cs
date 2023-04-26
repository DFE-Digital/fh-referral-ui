
namespace FamilyHubs.Referral.Core.Models;

public class ConnectionRequestModel
{
    public string? ServiceId { get; set; }
    public string? ServiceName { get; set; }
    public string? FamilyContactFullName { get; set; }
    public string? Reason { get; set; }
    public bool EmailSelected { get; set; }
    public bool TelephoneSelected { get; set; }
    public bool TextPhoneSelected { get; set; }
    public bool LetterSelected { get; set; }
}