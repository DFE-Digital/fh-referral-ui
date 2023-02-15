using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static FamilyHubs.ReferralUi.Ui.Infrastructure.Configuration.TempStorageConfiguration;


namespace FamilyHubs.ReferralUi.Ui.Services;

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
        _redisCache.SetStringValue(KeyOrgWithService, String.Empty, _timespanMinites);
    }

    public string RetrieveLastPageName()
    {
        return _redisCache.GetStringValue(KeyCurrentPage) ?? string.Empty;
    }

    public void StoreCurrentPageName(string? currPage)
    {
        if (currPage != null)
            _redisCache.SetStringValue(KeyCurrentPage, currPage, _timespanMinites);
    }

    public ServiceDto? RetrieveService()
    {
        return _redisCache.GetValue<ServiceDto>(KeyService);
    }

    public void StoreService(ServiceDto serviceDto)
    {
        _redisCache.SetValue<ServiceDto>(KeyService, serviceDto, _timespanMinites);
    }

    public void ResetLastPageName()
    {
        _redisCache.SetStringValue(KeyCurrentPage, String.Empty, _timespanMinites);
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
