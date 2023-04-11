using Ardalis.GuardClauses;
using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FamilyHubs.ReferralUi.UnitTests.Services;

public class WhenUsingTokenService
{
    

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
        
        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        int setCallback = 0;
        mockRedisCacheService.Setup(x => x.StoreStringValue(It.IsAny<string>(), It.IsAny<string>())).Callback(() => setCallback++);
        TokenService tokenService = new TokenService(mockRedisCacheService.Object);

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

        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        int setCallback = 0;
        mockRedisCacheService.Setup(x => x.StoreStringValue(It.IsAny<string>(), It.IsAny<string>())).Callback(() => setCallback++);
        TokenService tokenService = new TokenService(mockRedisCacheService.Object);

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
        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        int setCallback = 0;
        mockRedisCacheService.Setup(x => x.StoreStringValue(It.IsAny<string>(), It.IsAny<string>())).Callback(() => setCallback++);
        TokenService tokenService = new TokenService(mockRedisCacheService.Object);

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
        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        int setCallback = 0;
        mockRedisCacheService.Setup(x => x.RetrieveStringValue(It.IsAny<string>())).Callback(() => setCallback++).Returns(token);
        TokenService tokenService = new TokenService(mockRedisCacheService.Object);


        // Act
        string result = tokenService.GetToken();

        //Assert
        result.Should().BeEquivalentTo(token);
        setCallback.Should().Be(1);
    }

    [Fact]
    public void ThenGetTokenReturnsEmpty()
    {
        // Arrange
        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        int setCallback = 0;
        mockRedisCacheService.Setup(x => x.RetrieveStringValue(It.IsAny<string>())).Callback(() => setCallback++).Returns(string.Empty);
        TokenService tokenService = new TokenService(mockRedisCacheService.Object);

        // Act
        string result = tokenService.GetToken();

        //Assert
        result.Should().Be(string.Empty);
        setCallback.Should().Be(1);
    }

    [Fact]
    public void ThenGetRefeshToken()
    {
        // Arrange
        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        int setCallback = 0;
        mockRedisCacheService.Setup(x => x.RetrieveStringValue(It.IsAny<string>())).Callback(() => setCallback++).Returns("RefreshToken");
        TokenService tokenService = new TokenService(mockRedisCacheService.Object);

        // Act
        string result = tokenService.GetRefreshToken();

        //Assert
        result.Should().BeEquivalentTo("RefreshToken");
        setCallback.Should().Be(1);
    }

    [Fact]
    public void ThenGetRefreshTokenReturnsEmpty()
    {
        // Arrange
        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        int setCallback = 0;
        mockRedisCacheService.Setup(x => x.RetrieveStringValue(It.IsAny<string>())).Callback(() => setCallback++).Returns(string.Empty);
        TokenService tokenService = new TokenService(mockRedisCacheService.Object);


        // Act
        string result = tokenService.GetRefreshToken();

        //Assert
        result.Should().Be(string.Empty);
        setCallback.Should().Be(1);
    }

    

    [Fact]
    public void ThenGetUsersOrganisationId_ThrowsNotFoundException_WhenTokenDoesNotExist()
    {
        // Arrange
        int setCallback = 0;
        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        mockRedisCacheService.Setup(x => x.RetrieveStringValue(It.IsAny<string>())).Callback(() => setCallback++).Returns(string.Empty);
        TokenService tokenService = new TokenService(mockRedisCacheService.Object);

       

        // Act and Assert
        Assert.Throws<ArgumentException>(() => tokenService.GetUsersOrganisationId());
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
        int setCallback = 0;
        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        mockRedisCacheService.Setup(x => x.RetrieveStringValue(It.IsAny<string>())).Callback(() => setCallback++).Returns(token);
        TokenService tokenService = new TokenService(mockRedisCacheService.Object);

        

        // Act
        var result = tokenService.GetUsersOrganisationId();

        // Assert
        result.Should().Be("2d2124ea-3bb0-4802-b694-db02db5e7756");
        setCallback.Should().Be(1);

    }

    [Fact]
    public void ThenClearTokens()
    {
        // Arrange
        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        int setCallback = 0;
        mockRedisCacheService.Setup(x => x.StoreStringValue(It.IsAny<string>(), It.IsAny<string>())).Callback(() => setCallback++);
        TokenService tokenService = new TokenService(mockRedisCacheService.Object);

        //Act
        tokenService.ClearTokens();

        //Assert
        setCallback.Should().Be(2);
    }

    [Fact]
    public void ThenGetUserKey()
    {
        //Arrange
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
        int setCallback = 0;
        Mock<IRedisCacheService> mockRedisCacheService = new Mock<IRedisCacheService>();
        mockRedisCacheService.Setup(x => x.RetrieveStringValue(It.IsAny<string>())).Callback(() => setCallback++).Returns(token);
        TokenService tokenService = new TokenService(mockRedisCacheService.Object);


        //Act
        var result = tokenService.GetUserKey();

        //Assert
        result.Should().NotBeNull();
        result.Should().Be("ConnectWizzardViewModel-123");
        setCallback.Should().Be(1);
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
