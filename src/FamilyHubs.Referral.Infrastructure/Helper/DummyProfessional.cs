using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FamilyHubs.Referral.Infrastructure.Helper;

public static class DummyProfessional
{
    public static string GetProfessionalUserToken()
    {
        var authClaims = new List<Claim>
        {
                    new Claim("FirstName", "123"),
                    new Claim("LastName", "123"),
                    new Claim(ClaimTypes.Email, "Joe.Professional@email.com"),
                    new Claim("PhoneNumber", "0122 865 278"),
                    new Claim(ClaimTypes.Role, "Professional"),
                    new Claim("Team", "Social work team"),
                    new Claim("OrganisationId", "1"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        JwtSecurityToken tokenItem = CreateToken(authClaims);
        var token = new JwtSecurityTokenHandler().WriteToken(tokenItem);
        return token;
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

    public static List<Claim> GetClaimsFromToken(string? token = null)
    {
        if (token == null) 
        {
            token = GetProfessionalUserToken();
        }
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        return jwtSecurityToken.Claims.ToList();
    }
}
