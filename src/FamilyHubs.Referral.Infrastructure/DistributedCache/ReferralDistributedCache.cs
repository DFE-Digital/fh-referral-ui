using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace FamilyHubs.Referral.Infrastructure.DistributedCache;

public class ReferralDistributedCache : IReferralDistributedCache
{
    private readonly IDistributedCache _distributedCache;
    private readonly IReferralCacheKeys _referralCacheKeys;
    private readonly DistributedCacheEntryOptions _distributedCacheEntryOptions;

    public ReferralDistributedCache(
        IDistributedCache distributedCache,
        IReferralCacheKeys referralCacheKeys,
        DistributedCacheEntryOptions distributedCacheEntryOptions)
    {
        _distributedCache = distributedCache;
        _referralCacheKeys = referralCacheKeys;
        _distributedCacheEntryOptions = distributedCacheEntryOptions;
    }

    public async Task<ProfessionalReferralModel?> GetProfessionalReferralAsync()
    {
        return await _distributedCache.GetAsync<ProfessionalReferralModel>(_referralCacheKeys.ProfessionalReferral);
    }

    public async Task SetProfessionalReferralAsync(ProfessionalReferralModel model)
    {
        await _distributedCache.SetAsync(_referralCacheKeys.ProfessionalReferral, model, _distributedCacheEntryOptions);
    }
}