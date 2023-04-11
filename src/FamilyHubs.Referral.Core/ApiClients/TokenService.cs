using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;

namespace FamilyHubs.Referral.Core.ApiClients;

public interface ITokenService
{
    string GetToken();
    string GetRefreshToken();
    void SetToken(string tokenValue, DateTime validTo, string refreshToken);
    void ClearTokens();
    string GetUsersOrganisationId();
}

public class TokenService : ITokenService
{
    private readonly IMemoryCache _memoryCache;

    public TokenService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void SetToken(string tokenValue, DateTime validTo, string refreshToken)
    {
        if (string.IsNullOrEmpty(tokenValue) || string.IsNullOrEmpty(refreshToken))
            return;

        //api seems to be subtracting 60 mins
        TimeZoneInfo tzi = TimeZoneInfo.Local;
        TimeSpan offset = tzi.GetUtcOffset(validTo);
        DateTime validDate = DateTime.Now.Add(offset);

        if (!DateTime.Now.IsDaylightSavingTime())
        {
            validDate = validDate.AddHours(1);
        }

        TimeSpan ts = validDate - DateTime.Now;

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(ts);

        _memoryCache.Set("FamilyHubToken", tokenValue, cacheEntryOptions);
        _memoryCache.Set("FamilyHubRefreshToken", refreshToken, cacheEntryOptions);
    }

    public string GetToken()
    {
        if (_memoryCache.TryGetValue("FamilyHubToken", out string? cacheValue) && !string.IsNullOrEmpty(cacheValue))
        {
            return cacheValue;
        }

        return string.Empty;
    }
    public string GetRefreshToken()
    {
        if (_memoryCache.TryGetValue("FamilyHubRefreshToken", out string? cacheValue) && !string.IsNullOrEmpty(cacheValue))
        {
            return cacheValue;
        }

        return string.Empty;
    }

    public string GetUsersOrganisationId()
    {
        if (_memoryCache.TryGetValue("FamilyHubToken", out string? cacheValue) && !string.IsNullOrEmpty(cacheValue))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(cacheValue);
            var claims = jwtSecurityToken.Claims.ToList();

            var claim = claims.FirstOrDefault(x => x.Type == "OpenReferralOrganisationId");
            if (claim != null)
            {
                return claim.Value;
            }
        }

        throw new ArgumentException("OrganisationId not found");
    }

    public void ClearTokens()
    {
        _memoryCache.Remove("FamilyHubToken");
        _memoryCache.Remove("FamilyHubRefreshToken");
    }
}

