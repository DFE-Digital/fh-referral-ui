using FamilyHubs.Referral.Core.Models;
using FamilyHubs.ServiceDirectory.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FamilyHubs.Referral.Core.Services;

public class RedisCacheService : IDistributedCacheService
{
    private readonly IRedisCache _redisCache;
    private readonly string _sessionId;

    public RedisCacheService(IRedisCache redisCache, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _redisCache = redisCache;
        var session = httpContextAccessor.HttpContext!.Session;
        _sessionId = session.Id;
    }

    public void StoreConnectWizzardViewModel(string key, ConnectWizzardViewModel value)
    {
        _redisCache.SetStringValue($"{_sessionId}{key}", value.Encode());
    }

    //This will be used on the last page after the data is saved to the api
    public void ResetConnectWizzardViewModel(string key)
    {
        _redisCache.SetStringValue($"{_sessionId}{key}", string.Empty);
    }

    public ConnectWizzardViewModel RetrieveConnectWizzardViewModel(string key)
    {
        string value = _redisCache.GetStringValue($"{_sessionId}{key}") ?? string.Empty;
        if (string.IsNullOrEmpty(value))
        {
            return new ConnectWizzardViewModel();
        }

        ConnectWizzardViewModel? model = ConnectWizzardViewModel.Decode(value);
        ArgumentNullException.ThrowIfNull(model);
        return model;
    }
}

