using FamilyHubs.Referral.Core.Models;

namespace FamilyHubs.Referral.Core.DistributedCache;

public interface IReferralDistributedCache
{
    Task<ProfessionalReferralModel?> GetProfessionalReferralAsync();
    Task SetProfessionalReferralAsync(ProfessionalReferralModel model);
}