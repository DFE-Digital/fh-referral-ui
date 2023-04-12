using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FamilyHubs.Referral.Infrastructure;

//Tempory class for creating a Professional User

public static class DummyProfessional
{
    public static string GetProfessionalUserToken()
    {
        var authClaims = new List<Claim>
        {
                    new Claim("BtlPro", "123"),
                    new Claim(ClaimTypes.Email, "BtlPro@email.com"),
                    new Claim(ClaimTypes.Name, "Bristol Professional"),
                    new Claim(ClaimTypes.Role, "Professional"),
                    new Claim("OpenReferralOrganisationId", "1"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        JwtSecurityToken tokenItem = CreateToken(authClaims);
        var token = new JwtSecurityTokenHandler().WriteToken(tokenItem);
        return token;
    }

    public static async Task<long> LogUserIn(HttpContext httpContext, ITokenService tokenService, ClaimsPrincipal user, string token)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(tokenService);
        ArgumentNullException.ThrowIfNull(token);

        long organisationId = 0;

        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        var claims = jwtSecurityToken.Claims.ToList();

        var claim = claims.FirstOrDefault(x => x.Type == "OpenReferralOrganisationId");
        if (claim != null && long.TryParse(claim.Value, out long orgIdValue))
        {
            organisationId = orgIdValue;
        }

        var appIdentity = new ClaimsIdentity(claims);
        user.AddIdentity(appIdentity);

        //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme    
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
        var principal = new ClaimsPrincipal(identity);

        tokenService.SetToken(token, jwtSecurityToken.ValidTo, "RefreshToken");

        //SignInAsync is a Extension method for Sign in a principal for the specified scheme.    
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
        {
            IsPersistent = false //Input.RememberMe,
        });

        return organisationId;
    }

    public static async Task LogUserOut(HttpContext httpContext, ITokenService tokenService)
    {
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        tokenService.ClearTokens();
    }
    private static JwtSecurityToken CreateToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"));

        var token = new JwtSecurityToken(
            issuer: "https://localhost:7108",
            audience: "MySuperSecureApiUser",
            expires: DateTime.Now.AddMinutes(10),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        return token;
    }
}
