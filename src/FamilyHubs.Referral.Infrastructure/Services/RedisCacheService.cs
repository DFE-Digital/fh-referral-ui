using FamilyHubs.Referral.Core.Models;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FamilyHubs.Referral.Core.Services;

public class RedisCacheService : IDistributedCacheService
{
    private readonly IRedisCache _redisCache;
    private readonly int _timespanMinites;
    private readonly string _sessionId;

    public RedisCacheService(IRedisCache redisCache, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _redisCache = redisCache;
        _timespanMinites = configuration.GetValue<int>("SessionTimeOutMinutes");
        var session = httpContextAccessor.HttpContext!.Session;
        _sessionId = session.Id;
    }

    public void ResetOrganisationWithService()
    {
        _redisCache.SetStringValue($"{_sessionId}{TempStorageConfiguration.KeyOrgWithService}", String.Empty, _timespanMinites);
    }

    public string RetrieveLastPageName()
    {
        return _redisCache.GetStringValue($"{_sessionId}{TempStorageConfiguration.KeyCurrentPage}") ?? string.Empty;
    }

    public void StoreCurrentPageName(string? currPage)
    {
        if (currPage != null)
            _redisCache.SetStringValue($"{_sessionId}{TempStorageConfiguration.KeyCurrentPage}", currPage, _timespanMinites);
    }

    public ServiceDto? RetrieveService()
    {
        return _redisCache.GetValue<ServiceDto>($"{_sessionId}{TempStorageConfiguration.KeyService}");
    }

    public void StoreService(ServiceDto serviceDto)
    {
        _redisCache.SetValue<ServiceDto>($"{_sessionId}{TempStorageConfiguration.KeyService}", serviceDto, _timespanMinites);
    }

    public void ResetLastPageName()
    {
        _redisCache.SetStringValue($"{_sessionId}{TempStorageConfiguration.KeyCurrentPage}", String.Empty, _timespanMinites);
    }

    public void StoreConnectWizzardViewModel(string key, ConnectWizzardViewModel value)
    {
        _redisCache.SetStringValue($"{_sessionId}{key}", value.Encode());
    }

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

    public void StoreStringValue(string key, string value)
    {
        _redisCache.SetStringValue($"{_sessionId}{key}", value);
    }

    public string RetrieveStringValue(string key)
    {
        try
        {
            return _redisCache.GetStringValue($"{_sessionId}{key}") ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
        
    }
}

