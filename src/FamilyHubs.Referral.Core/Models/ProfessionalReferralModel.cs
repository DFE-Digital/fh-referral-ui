
namespace FamilyHubs.Referral.Core.Models;

public class ProfessionalReferralModel
{
    public string ServiceId { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the contact
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    public string ReasonForSupport { get; set; } = string.Empty;
}