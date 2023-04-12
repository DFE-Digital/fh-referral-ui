using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Helpers;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace FamilyHubs.Referral.Core.Services;

public class RedisCacheService : IDistributedCacheService
{
    private readonly IRedisCache _redisCache;
    private readonly int _timespanMinites;

    public RedisCacheService(IRedisCache redisCache, IConfiguration configuration)
    {
        _redisCache = redisCache;
        _timespanMinites = configuration.GetValue<int>("SessionTimeOutMinutes");
    }

    public void ResetOrganisationWithService()
    {
        _redisCache.SetStringValue(TempStorageConfiguration.KeyOrgWithService, String.Empty, _timespanMinites);
    }

    public string RetrieveLastPageName()
    {
        return _redisCache.GetStringValue(TempStorageConfiguration.KeyCurrentPage) ?? string.Empty;
    }

    public void StoreCurrentPageName(string? currPage)
    {
        if (currPage != null)
            _redisCache.SetStringValue(TempStorageConfiguration.KeyCurrentPage, currPage, _timespanMinites);
    }

    public ServiceDto? RetrieveService()
    {
        return _redisCache.GetValue<ServiceDto>(TempStorageConfiguration.KeyService);
    }

    public void StoreService(ServiceDto serviceDto)
    {
        _redisCache.SetValue<ServiceDto>(TempStorageConfiguration.KeyService, serviceDto, _timespanMinites);
    }

    public void ResetLastPageName()
    {
        _redisCache.SetStringValue(TempStorageConfiguration.KeyCurrentPage, String.Empty, _timespanMinites);
    }

    public void StoreConnectWizzardViewModel(string key, ConnectWizzardViewModel value)
    {
        _redisCache.SetStringValue(key, value.Encode());
    }

    public void ResetConnectWizzardViewModel(string key)
    {
        _redisCache.SetStringValue(key, string.Empty);
    }

    public ConnectWizzardViewModel RetrieveConnectWizzardViewModel(string key)
    {
        string value = _redisCache.GetStringValue($"{key}") ?? string.Empty;
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
        _redisCache.SetStringValue(key, value);
    }

    public string RetrieveStringValue(string key)
    {
        try
        {
            return _redisCache.GetStringValue(key);
        }
        catch
        {
            return string.Empty;
        }
        
    }
}

