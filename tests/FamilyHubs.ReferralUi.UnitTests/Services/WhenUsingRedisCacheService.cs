using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FamilyHubs.ReferralUi.UnitTests.Services;

public class WhenUsingRedisCacheService
{
    private readonly Mock<IRedisCache> _mockRedisCache;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly IRedisCacheService _redisCacheService;
    public WhenUsingRedisCacheService()
    {
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("SessionTimeOutMinutes", "30")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        _mockRedisCache = new Mock<IRedisCache>();
        _mockTokenService = new Mock<ITokenService>();

        _redisCacheService = new RedisCacheService(_mockRedisCache.Object, configuration, _mockTokenService.Object);
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
        _mockTokenService.Setup(x => x.GetToken()).Returns(token);

        //Act
        var result = _redisCacheService.GetUserKey();

        //Assert
        result.Should().NotBeNull();
        result.Should().Be("ConnectWizzardViewModel-123");
    }

    [Fact]
    public void ThenResetOrganisationWithService()
    {
        //Arrange
        int setStringCallback = 0;
        _mockRedisCache.Setup(x => x.SetStringValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Callback(() => setStringCallback++);

        //Act
        _redisCacheService.ResetOrganisationWithService();

        //Assert
        setStringCallback.Should().Be(1);
    }

    [Fact]
    public void ThenRetrieveLastPageName()
    {
        //Arrange
        _mockRedisCache.Setup(x => x.GetStringValue(It.IsAny<string>())).Returns("/LastPage");

        //Act
        string result = _redisCacheService.RetrieveLastPageName();

        //Assert
        result.Should().Be("/LastPage");
    }

    [Fact]
    public void ThenStoreCurrentPageName()
    {
        //Arrange
        int setStringCallback = 0;
        _mockRedisCache.Setup(x => x.SetStringValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Callback(() => setStringCallback++);

        //Act
        _redisCacheService.StoreCurrentPageName("currentPage");

        //Assert
        setStringCallback.Should().Be(1);
    }

    [Fact]
    public void ThenRetrieveService()
    {
        //Arrange
        ServiceDto service = BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString());
        _mockRedisCache.Setup(x => x.GetValue<ServiceDto>(It.IsAny<string>())).Returns(service);

        //Act
        ServiceDto? result = _redisCacheService.RetrieveService();

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(service);
    }

    [Fact]
    public void ThenStoreService()
    {
        //Arrange
        ServiceDto service = BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString());
        int setStringCallback = 0;
        _mockRedisCache.Setup(x => x.SetValue<ServiceDto>(It.IsAny<string>(), It.IsAny<ServiceDto>(), It.IsAny<int>())).Callback(() => setStringCallback++);

        //Act
        _redisCacheService.StoreService(service);

        //Assert
        setStringCallback.Should().Be(1);
    }

    [Fact]
    public void ThenResetLastPageName()
    {
        //Arrange
        int setStringCallback = 0;
        _mockRedisCache.Setup(x => x.SetStringValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Callback(() => setStringCallback++);

        //Act
        _redisCacheService.ResetLastPageName();

        //Assert
        setStringCallback.Should().Be(1);
    }

    [Fact]
    public void ThenStoreConnectWizzardViewModel()
    {
        //Arrange
        int setStringCallback = 0;
        _mockRedisCache.Setup(x => x.SetStringValue(It.IsAny<string>(), It.IsAny<string>())).Callback(() => setStringCallback++);

        //Act
        _redisCacheService.StoreConnectWizzardViewModel("key", new ConnectWizzardViewModel());

        //Assert
        setStringCallback.Should().Be(1);
    }

    [Fact]
    public void ThenResetConnectWizzardViewModel()
    {
        //Arrange
        int setStringCallback = 0;
        _mockRedisCache.Setup(x => x.SetStringValue(It.IsAny<string>(), It.IsAny<string>())).Callback(() => setStringCallback++);

        //Act
        _redisCacheService.ResetConnectWizzardViewModel("currentModel");

        //Assert
        setStringCallback.Should().Be(1);
    }

    [Fact]
    public void ThenRetrieveConnectWizzardViewModelWithNullKey()
    {
        //Arrange
        ConnectWizzardViewModel expectedResult = new();

        //Act
        ConnectWizzardViewModel result = _redisCacheService.RetrieveConnectWizzardViewModel(default!);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void ThenRetrieveConnectWizzardViewModelWhichHasBeenEncoded()
    {
        ConnectWizzardViewModel expectedResult = new()
        {
            ReferralId = Guid.NewGuid().ToString(),
            ServiceId = Guid.NewGuid().ToString(),
            ServiceName = "ServiceName",
            FullName = "Fullname",
            Telephone = "Telephone",
            Textphone = "Textphone",
            EmailAddress = "EmailAddress",
            AnyoneInFamilyBeingHarmed = false,
            HaveConcent = true,
            ReasonForSupport = "ReasonForSupport"
        };
        _mockRedisCache.Setup(x => x.GetStringValue(It.IsAny<string>())).Returns(expectedResult.Encode());

        ConnectWizzardViewModel result = _redisCacheService.RetrieveConnectWizzardViewModel("key");

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
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