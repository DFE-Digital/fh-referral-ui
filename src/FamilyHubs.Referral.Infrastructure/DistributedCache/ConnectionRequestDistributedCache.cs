using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace FamilyHubs.Referral.Infrastructure.DistributedCache;

public class ConnectionRequestDistributedCache : IConnectionRequestDistributedCache
{
    private readonly IDistributedCache _distributedCache;
    private readonly ICacheKeys _cacheKeys;
    private readonly DistributedCacheEntryOptions _distributedCacheEntryOptions;

    public ConnectionRequestDistributedCache(
        IDistributedCache distributedCache,
        ICacheKeys cacheKeys,
        DistributedCacheEntryOptions distributedCacheEntryOptions)
    {
        _distributedCache = distributedCache;
        _cacheKeys = cacheKeys;
        _distributedCacheEntryOptions = distributedCacheEntryOptions;
    }

    public async Task<ConnectionRequestModel?> GetAsync()
    {
        return await _distributedCache.GetAsync<ConnectionRequestModel>(_cacheKeys.ConnectionRequest);
    }

    public async Task SetAsync(ConnectionRequestModel model)
    {
        await _distributedCache.SetAsync(_cacheKeys.ConnectionRequest, model, _distributedCacheEntryOptions);
    }

    public async Task RemoveAsync()
    {
        await _distributedCache.RemoveAsync(_cacheKeys.ConnectionRequest);
    }
}