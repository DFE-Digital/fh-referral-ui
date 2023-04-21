//using FamilyHubs.Referral.Core.Models;
//using FamilyHubs.Referral.Core.Services;
//using FamilyHubs.ServiceDirectory.Shared.Helpers;
//using FluentAssertions;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Moq;

//namespace FamilyHubs.ReferralUi.UnitTests.Services;

//public class WhenUsingRedisCacheService
//{
//    private readonly Mock<IRedisCache> _mockRedisCache;
//    private readonly IDistributedCacheService _redisCacheService;
//    public WhenUsingRedisCacheService()
//    {
//        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
//        {
//            new KeyValuePair<string, string?>("SessionTimeOutMinutes", "30")
//        };

//        IConfiguration configuration = new ConfigurationBuilder()
//            .AddInMemoryCollection(inMemorySettings)
//            .Build();
//        _mockRedisCache = new Mock<IRedisCache>();

//        Mock<ISession> mockSession = new Mock<ISession>();
//        var httpContext = new DefaultHttpContext() { Session = mockSession.Object };
//        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
//        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

//        _redisCacheService = new RedisCacheService(_mockRedisCache.Object, configuration, mockHttpContextAccessor.Object);
//    }

//    [Fact]
//    public void ThenStoreConnectWizzardViewModel()
//    {
//        //Arrange
//        int setStringCallback = 0;
//        _mockRedisCache.Setup(x => x.SetStringValue(It.IsAny<string>(), It.IsAny<string>())).Callback(() => setStringCallback++);

//        //Act
//        _redisCacheService.StoreConnectWizzardViewModel("key", new ProfessionalReferralModel());

//        //Assert
//        setStringCallback.Should().Be(1);
//    }

//    [Fact]
//    public void ThenResetConnectWizzardViewModel()
//    {
//        //Arrange
//        int setStringCallback = 0;
//        _mockRedisCache.Setup(x => x.SetStringValue(It.IsAny<string>(), It.IsAny<string>())).Callback(() => setStringCallback++);

//        //Act
//        _redisCacheService.ResetConnectWizzardViewModel("currentModel");

//        //Assert
//        setStringCallback.Should().Be(1);
//    }

//    [Fact]
//    public void ThenRetrieveConnectWizzardViewModelWithNullKey()
//    {
//        //Arrange
//        ProfessionalReferralModel expectedResult = new();

//        //Act
//        ProfessionalReferralModel result = _redisCacheService.RetrieveConnectWizzardViewModel(default!);

//        //Assert
//        result.Should().NotBeNull();
//        result.Should().BeEquivalentTo(expectedResult);
//    }

//    [Fact]
//    public void ThenRetrieveConnectWizzardViewModelWhichHasBeenEncoded()
//    {
//        ProfessionalReferralModel expectedResult = new()
//        {
//            ServiceId = Guid.NewGuid().ToString(),
//            ServiceName = "ServiceName",
//            FullName = "Fullname",
//        };
//        _mockRedisCache.Setup(x => x.GetStringValue(It.IsAny<string>())).Returns(expectedResult.Encode());

//        ProfessionalReferralModel result = _redisCacheService.RetrieveConnectWizzardViewModel("key");

//        result.Should().NotBeNull();
//        result.Should().BeEquivalentTo(expectedResult);
//    }
//}
