using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Helpers;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace FamilyHubs.Referral.Core.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IRedisCache _redisCache;
    private readonly int _timespanMinites;
    private readonly ITokenService _tokenService;

    public RedisCacheService(IRedisCache redisCache, IConfiguration configuration, ITokenService tokenService)
    {
        _redisCache = redisCache;
        _timespanMinites = configuration.GetValue<int>("SessionTimeOutMinutes");
        _tokenService = tokenService;
    }

    public string GetUserKey()
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(_tokenService.GetToken());
        var claims = jwtSecurityToken.Claims.ToList();
        var claim = claims.FirstOrDefault(x => x.Type == "UserId");
        ArgumentNullException.ThrowIfNull(claim);
        return $"ConnectWizzardViewModel-{claim.Value}";
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

    void IRedisCacheService.StoreConnectWizzardViewModel(string key, ConnectWizzardViewModel value)
    {
        _redisCache.SetStringValue(key, value.Encode());
    }

    void IRedisCacheService.ResetConnectWizzardViewModel(string key)
    {
        _redisCache.SetStringValue(key, string.Empty);
    }

    ConnectWizzardViewModel IRedisCacheService.RetrieveConnectWizzardViewModel(string key)
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
}

