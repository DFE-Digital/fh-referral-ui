using FamilyHubs.ReferralUi.Ui.Infrastructure.Configuration;
using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using static FamilyHubs.ReferralUi.Ui.Infrastructure.Configuration.TempStorageConfiguration;


namespace FamilyHubs.ReferralUi.Ui.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly string _sessionId;
    private readonly TimeSpan _timeSpanMinutes;
    private readonly ITokenService _tokenService;

    public CacheService(IMemoryCache cache, IConfiguration configuration, ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
    {
        _cache = cache;
        var timeoutValue = configuration.GetValue<int?>("SessionTimeOutMinutes");
        ArgumentNullException.ThrowIfNull(timeoutValue);

        _timeSpanMinutes = TimeSpan.FromMinutes(timeoutValue.Value);


        _tokenService = tokenService;
        var session = httpContextAccessor.HttpContext!.Session;
        _sessionId = session.Id;
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
        _cache.Set($"{_sessionId}{KeyOrgWithService}", String.Empty, _timeSpanMinutes);
    }

    public string RetrieveLastPageName()
    {
        return _cache.Get<string>($"{_sessionId}{KeyCurrentPage}") ?? string.Empty;
    }

    public void StoreCurrentPageName(string? currPage)
    {
        if (currPage != null)
            _cache.Set($"{_sessionId}{KeyCurrentPage}", currPage, _timeSpanMinutes);
    }

    public ServiceDto? RetrieveService()
    {
        return _cache.Get<ServiceDto>($"{_sessionId}{KeyService}");
    }

    public void StoreService(ServiceDto serviceDto)
    {
        _cache.Set<ServiceDto>($"{_sessionId}{KeyService}", serviceDto, _timeSpanMinutes);
    }

    public void ResetLastPageName()
    {
        _cache.Set($"{_sessionId}{KeyCurrentPage}", String.Empty, _timeSpanMinutes);
    }

    void ICacheService.StoreConnectWizzardViewModel(string key, ConnectWizzardViewModel value)
    {
        _cache.Set($"{_sessionId}{key}", value.Encode());
    }

    void ICacheService.ResetConnectWizzardViewModel(string key)
    {
        _cache.Set($"{_sessionId}{key}", string.Empty);
    }

    ConnectWizzardViewModel ICacheService.RetrieveConnectWizzardViewModel(string key)
    {
        string value = _cache.Get<string>($"{_sessionId}{key}") ?? string.Empty;
        if (string.IsNullOrEmpty(value))
        {
            return new ConnectWizzardViewModel();
        }

        ConnectWizzardViewModel? model = ConnectWizzardViewModel.Decode(value);
        ArgumentNullException.ThrowIfNull(model);
        return model;
    }
}



