using Ardalis.GuardClauses;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FamilyHubs.ReferralUi.UnitTests.Services;

public class WhenUsingTokenService
{
    private Mock<IMemoryCache> _mockMemoryCache = default!;

    [Fact]
    public void ThenSetToken()
    {
        // Arrange
        var authClaims = new List<Claim>
        {
                    new Claim("UserId", "123"),
                    new Claim(ClaimTypes.Name, "TestUser"),
                    new Claim(ClaimTypes.Role, "Role"),
                    new Claim("OpenReferralOrganisationId", "2d2124ea-3bb0-4802-b694-db02db5e7756"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        JwtSecurityToken tokenItem = CreateToken(authClaims);
        var token = new JwtSecurityTokenHandler().WriteToken(tokenItem);
        IMemoryCache memoryCache = GetMemoryCache(token);
        TokenService tokenService = new TokenService(memoryCache);
        int setCallback = 0;
        _mockMemoryCache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Callback(() => setCallback++)
            .Returns(Mock.Of<ICacheEntry>);

        
        // Act
        tokenService.SetToken(token, DateTime.UtcNow, "RefreshTestToken");

        //Assert
        setCallback.Should().Be(2);
    }

    [Fact]
    public void ThenSetTokenFailsWithEmptyToken()
    {// Arrange
        var authClaims = new List<Claim>
        {
                    new Claim("UserId", "123"),
                    new Claim(ClaimTypes.Name, "TestUser"),
                    new Claim(ClaimTypes.Role, "Role"),
                    new Claim("OpenReferralOrganisationId", "2d2124ea-3bb0-4802-b694-db02db5e7756"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        JwtSecurityToken tokenItem = CreateToken(authClaims);
        var token = new JwtSecurityTokenHandler().WriteToken(tokenItem);
        IMemoryCache memoryCache = GetMemoryCache(token);
        TokenService tokenService = new TokenService(memoryCache);
        int setCallback = 0;
        _mockMemoryCache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Callback(() => setCallback++)
            .Returns(Mock.Of<ICacheEntry>);


        // Act
        tokenService.SetToken(default!, DateTime.UtcNow, "RefreshTestToken");

        //Assert
        setCallback.Should().Be(0);
    }

    [Fact]
    public void ThenSetTokenFailsWithEmptyRefreshToken()
    {// Arrange
        var authClaims = new List<Claim>
        {
                    new Claim("UserId", "123"),
                    new Claim(ClaimTypes.Name, "TestUser"),
                    new Claim(ClaimTypes.Role, "Role"),
                    new Claim("OpenReferralOrganisationId", "2d2124ea-3bb0-4802-b694-db02db5e7756"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        JwtSecurityToken tokenItem = CreateToken(authClaims);
        var token = new JwtSecurityTokenHandler().WriteToken(tokenItem);
        IMemoryCache memoryCache = GetMemoryCache(token);
        TokenService tokenService = new TokenService(memoryCache);
        int setCallback = 0;
        _mockMemoryCache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Callback(() => setCallback++)
            .Returns(Mock.Of<ICacheEntry>);


        // Act
        tokenService.SetToken(token, DateTime.UtcNow, default!);

        //Assert
        setCallback.Should().Be(0);
    }

    [Fact]
    public void ThenGetToken()
    {
        // Arrange
        var authClaims = new List<Claim>
        {
                    new Claim("UserId", "123"),
                    new Claim(ClaimTypes.Name, "TestUser"),
                    new Claim(ClaimTypes.Role, "Role"),
                    new Claim("OpenReferralOrganisationId", "2d2124ea-3bb0-4802-b694-db02db5e7756"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        JwtSecurityToken tokenItem = CreateToken(authClaims);
        var token = new JwtSecurityTokenHandler().WriteToken(tokenItem);
        IMemoryCache memoryCache = GetMemoryCache(token);
        TokenService tokenService = new TokenService(memoryCache);


        // Act
        string result = tokenService.GetToken();

        //Assert
        result.Should().BeEquivalentTo(token);
    }

    [Fact]
    public void ThenGetTokenReturnsEmpty()
    {
        // Arrange
        var authClaims = new List<Claim>
        {
                    new Claim("UserId", "123"),
                    new Claim(ClaimTypes.Name, "TestUser"),
                    new Claim(ClaimTypes.Role, "Role"),
                    new Claim("OpenReferralOrganisationId", "2d2124ea-3bb0-4802-b694-db02db5e7756"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        JwtSecurityToken tokenItem = CreateToken(authClaims);
        var token = new JwtSecurityTokenHandler().WriteToken(tokenItem);
        IMemoryCache memoryCache = GetMemoryCache(token, false);

        TokenService tokenService = new TokenService(memoryCache);
        
        // Act
        string result = tokenService.GetToken();

        //Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ThenGetRefeshToken()
    {
        // Arrange
        IMemoryCache memoryCache = GetMemoryCache("RefreshToken");
        TokenService tokenService = new TokenService(memoryCache);


        // Act
        string result = tokenService.GetRefreshToken();

        //Assert
        result.Should().BeEquivalentTo("RefreshToken");
    }

    [Fact]
    public void ThenGetRefreshTokenReturnsEmpty()
    {
        // Arrange
        IMemoryCache memoryCache = GetMemoryCache("RefreshToken",false);
        TokenService tokenService = new TokenService(memoryCache);


        // Act
        string result = tokenService.GetRefreshToken();

        //Assert
        result.Should().Be(string.Empty);
    }
    

    [Fact]
    public void ThenGetUsersOrganisationId_ThrowsNotFoundException_WhenTokenDoesNotExist()
    {
        // Arrange
        var authClaims = new List<Claim>
        {
                    new Claim("UserId", "123"),
                    new Claim(ClaimTypes.Name, "TestUser"),
                    new Claim(ClaimTypes.Role, "Role"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        JwtSecurityToken tokenItem = CreateToken(authClaims);
        var token = new JwtSecurityTokenHandler().WriteToken(tokenItem);
        TokenService tokenService = new TokenService(GetMemoryCache(token));

        // Act and Assert
        Assert.Throws<NotFoundException>(() => tokenService.GetUsersOrganisationId());
    }

    [Fact]
    public void GetUsersOrganisationId_ReturnsOrganisationId_WhenTokenExists()
    {
        // Arrange
        var authClaims = new List<Claim>
        {
                    new Claim("UserId", "123"),
                    new Claim(ClaimTypes.Name, "TestUser"),
                    new Claim(ClaimTypes.Role, "Role"),
                    new Claim("OpenReferralOrganisationId", "2d2124ea-3bb0-4802-b694-db02db5e7756"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        JwtSecurityToken tokenItem = CreateToken(authClaims);
        var token = new JwtSecurityTokenHandler().WriteToken(tokenItem);
        TokenService tokenService = new TokenService(GetMemoryCache(token));
        
        // Act
        var result = tokenService.GetUsersOrganisationId();

        // Assert
        result.Should().Be("2d2124ea-3bb0-4802-b694-db02db5e7756");
        
    }

    [Fact]
    public void ThenClearTokens()
    {
        // Arrange
        Mock<IMemoryCache> mockMemoryCache = new Mock<IMemoryCache>();
        int callback = 0;
        mockMemoryCache.Setup(x => x.Remove(It.IsAny<string>()))
            .Callback(() => callback++);
        TokenService tokenService = new TokenService(mockMemoryCache.Object);

        //Act
        tokenService.ClearTokens();

        //Assert
        callback.Should().Be(2);
    }

    public IMemoryCache GetMemoryCache(object expectedValue, bool retVal = true)
    {
        
        _mockMemoryCache = new Mock<IMemoryCache>();
#pragma warning disable CS8600 // Possible null reference assignment.
        _mockMemoryCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedValue))
            .Returns(retVal);
#pragma warning restore CS8600 // Possible null reference assignment.
        return _mockMemoryCache.Object;
    }

    
    private JwtSecurityToken CreateToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"));
        
        var token = new JwtSecurityToken(
            issuer: "https://localhost:7108",
            audience: "MySuperSecureApiUser",
            expires: DateTime.Now.AddMinutes(5),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        return token;
    }

}
