using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.Services;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net.Http;

namespace FamilyHubs.ReferralUi.UnitTests.Services;

public class WhenUsingRedisCacheService
{
    private readonly Mock<IRedisCache> _mockRedisCache;
    private readonly IDistributedCacheService _redisCacheService;
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

        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);

        var mockSession = new Mock<ISession>();
        mockSession.Setup(x => x.Id).Returns("SessionId");
        var httpContext = new DefaultHttpContext()
        {
            Session = mockSession.Object,
        };
        mockHttpContextAccessor.SetupGet(accessor => accessor.HttpContext).Returns(httpContext);

        _redisCacheService = new RedisCacheService(_mockRedisCache.Object, configuration, mockHttpContextAccessor.Object);
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
        ServiceDto service = BaseClientService.GetTestCountyCouncilServicesDto(1);
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
        ServiceDto service = BaseClientService.GetTestCountyCouncilServicesDto(1);
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
            //ReferralId = Guid.NewGuid().ToString(),
            ServiceId = Guid.NewGuid().ToString(),
            ServiceName = "ServiceName",
            //FullName = "Fullname",
            //Telephone = "Telephone",
            //Textphone = "Textphone",
            //EmailAddress = "EmailAddress",
            //AnyoneInFamilyBeingHarmed = false,
            HaveConcent = true,
            //ReasonForSupport = "ReasonForSupport"
        };
        _mockRedisCache.Setup(x => x.GetStringValue(It.IsAny<string>())).Returns(expectedResult.Encode());

        ConnectWizzardViewModel result = _redisCacheService.RetrieveConnectWizzardViewModel("key");

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void ThenRetrieveStringValue()
    {
        //Arrange
        _mockRedisCache.Setup(x => x.GetStringValue(It.IsAny<string>())).Returns("TestValue");

        //Act
        string result = _redisCacheService.RetrieveStringValue("Key");

        //Assert
        result.Should().Be("TestValue");
    }

    [Fact]
    public void ThenStoreStringValue()
    {
        //Arrange
        int setStringCallback = 0;
        _mockRedisCache.Setup(x => x.SetStringValue(It.IsAny<string>(), It.IsAny<string>())).Callback(() => setStringCallback++);

        //Act
        _redisCacheService.StoreStringValue("Key", "TestValue");

        //Assert
        setStringCallback.Should().Be(1);
    }
}
