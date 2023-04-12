using FamilyHubs.Referral.Core.Services;
using System.IdentityModel.Tokens.Jwt;

namespace FamilyHubs.Referral.Core.ApiClients;


public class TokenService : ITokenService
{
    private readonly IDistributedCacheService _redisCacheService;

    public TokenService(IDistributedCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }

    public void SetToken(string tokenValue, DateTime validTo, string refreshToken)
    {
        if (string.IsNullOrEmpty(tokenValue) || string.IsNullOrEmpty(refreshToken))
            return;

        _redisCacheService.StoreStringValue("FamilyHubToken", tokenValue);
        _redisCacheService.StoreStringValue("FamilyHubRefreshToken", refreshToken);
    }

    public string GetToken()
    {
        return _redisCacheService.RetrieveStringValue("FamilyHubToken");
    }
    public string GetRefreshToken()
    {
        string refreshToken = _redisCacheService.RetrieveStringValue("FamilyHubRefreshToken");
        if (!string.IsNullOrEmpty(refreshToken))
            return refreshToken;

        return string.Empty;
    }

    public string GetUsersOrganisationId()
    {
        string token = _redisCacheService.RetrieveStringValue("FamilyHubToken");
        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
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
        _redisCacheService.StoreStringValue("FamilyHubToken", default!);
        _redisCacheService.StoreStringValue("FamilyHubRefreshToken", default!);
    }

    public string GetUserKey()
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(GetToken());
        var claims = jwtSecurityToken.Claims.ToList();
        var claim = claims.FirstOrDefault(x => x.Type == "UserId");
        ArgumentNullException.ThrowIfNull(claim);
        return $"ConnectWizzardViewModel-{claim.Value}";
    }
}

