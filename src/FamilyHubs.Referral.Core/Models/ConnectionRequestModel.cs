
namespace FamilyHubs.Referral.Core.Models;

public class ConnectionRequestModel
{
    public string ServiceId { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string FamilyContactFullName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}