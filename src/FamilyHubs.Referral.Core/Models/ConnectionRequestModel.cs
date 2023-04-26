
namespace FamilyHubs.Referral.Core.Models;

public class ConnectionRequestModel
{
    public string? ServiceId { get; set; }
    public string? ServiceName { get; set; }
    public string? FamilyContactFullName { get; set; }
    public string? Reason { get; set; }
}