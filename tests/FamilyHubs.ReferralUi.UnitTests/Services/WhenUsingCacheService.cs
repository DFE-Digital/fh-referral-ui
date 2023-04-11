using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static FamilyHubs.ReferralUi.Ui.Infrastructure.Configuration.TempStorageConfiguration;

namespace FamilyHubs.ReferralUi.UnitTests.Services;

class TestSession : ISession
{

    public TestSession()
    {
        Values = new Dictionary<string, byte[]>();
    }

    public string Id
    {
        get
        {
            return "session_id";
        }
    }

    public bool IsAvailable
    {
        get
        {
            return true;
        }
    }

    public IEnumerable<string> Keys
    {
        get { return Values.Keys; }
    }

    public Dictionary<string, byte[]> Values { get; set; }

    public void Clear()
    {
        Values.Clear();
    }

    public Task CommitAsync()
    {
        throw new NotImplementedException();
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task LoadAsync()
    {
        throw new NotImplementedException();
    }

    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Remove(string key)
    {
        Values.Remove(key);
    }

    public void Set(string key, byte[] value)
    {
        if (Values.ContainsKey(key))
        {
            Remove(key);
        }
        Values.Add(key, value);
    }

    public bool TryGetValue(string key, out byte[] value)
    {
        if (Values.ContainsKey(key))
        {
            value = Values[key];
            return true;
        }
        value = new byte[0];
        return false;
    }
}

public class WhenUsingCacheService
{
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly ICacheService _cacheService;
    private readonly IMemoryCache _memoryCache;
    public WhenUsingCacheService()
    {
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("SessionTimeOutMinutes", "30")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
      
        _mockTokenService = new Mock<ITokenService>();

        Mock<IHttpContextAccessor> mockIHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var defaultHttpContext = new DefaultHttpContext();
        defaultHttpContext.Session = new TestSession();
        mockIHttpContextAccessor.Setup(x => x.HttpContext).Returns(defaultHttpContext);
        _memoryCache = GetMemoryCache();

        _cacheService = new CacheService(_memoryCache, configuration, _mockTokenService.Object, mockIHttpContextAccessor.Object);
    }

    public IMemoryCache GetMemoryCache()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        var serviceProvider = services.BuildServiceProvider();

        var memoryCache = serviceProvider.GetService<IMemoryCache>();
        return memoryCache ?? default!;
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
        var result = _cacheService.GetUserKey();

        //Assert
        result.Should().NotBeNull();
        result.Should().Be("ConnectWizzardViewModel-123");
    }

    [Fact]
    public void ThenResetOrganisationWithService()
    {
        //Arrange
        ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString());
        _memoryCache.Set<ServiceDto>($"session_id{KeyOrgWithService}", serviceDto, new TimeSpan(0,1,0));

        //Act
        _cacheService.ResetOrganisationWithService();
        var result = _memoryCache.Get<ServiceDto>($"session_id{KeyService}");

        //Assert
        result.Should().BeNull();
        
    }

    [Fact]
    public void ThenRetrieveLastPageName()
    {
        //Arrange
        _memoryCache.Set($"session_id{KeyCurrentPage}", "/LastPage", new TimeSpan(0, 1, 0));
        

        //Act
        string result = _cacheService.RetrieveLastPageName();

        //Assert
        result.Should().Be("/LastPage");
    }

    [Fact]
    public void ThenStoreCurrentPageName() 
    {
        //Arrange and Act
        _cacheService.StoreCurrentPageName("currentPage");
        var result = _memoryCache.Get<string>($"session_id{KeyCurrentPage}");

        //Assert
        result.Should().Be("currentPage");
    }

    [Fact]
    public void ThenRetrieveService()
    {
        //Arrange
        ServiceDto service = BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString());
        _memoryCache.Set<ServiceDto>($"session_id{KeyService}", service, new TimeSpan(0, 1, 0));

        //Act
        ServiceDto? result = _cacheService.RetrieveService();

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(service);
    }

    [Fact]
    public void ThenStoreService()
    {
        //Arrange
        ServiceDto service = BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString());
        
        //Act
        _cacheService.StoreService(service);
        var result = _memoryCache.Get<ServiceDto>($"session_id{KeyService}");

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(service);
    }

    [Fact]
    public void ThenResetLastPageName()
    {
        //Arrange
        _memoryCache.Set($"session_id{KeyCurrentPage}", "/LastPage", new TimeSpan(0, 1, 0));
        

        //Act
        _cacheService.ResetLastPageName();
        var result = _memoryCache.Get($"session_id{KeyCurrentPage}");

        //Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ThenStoreConnectWizzardViewModel()
    {
        //Arrange
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
        _memoryCache.Set<string>($"session_idkey", expectedResult.Encode(), new TimeSpan(0, 1, 0));

        

        //Act
        _cacheService.StoreConnectWizzardViewModel("key", expectedResult);
        var model = _memoryCache.Get<string>($"session_idkey");
        ConnectWizzardViewModel result = ConnectWizzardViewModel.Decode( model ) ?? default!;

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void ThenResetConnectWizzardViewModel()
    {
        //Arrange
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
        _memoryCache.Set<string>($"session_idkey", expectedResult.Encode(), new TimeSpan(0, 1, 0));

        //Act
        _cacheService.ResetConnectWizzardViewModel("key");
        var model = _memoryCache.Get<string>($"session_idkey");

        //Assert
        model.Should().BeEmpty();
    }

    [Fact]
    public void ThenRetrieveConnectWizzardViewModelWithNullKey()
    {
        //Arrange
        ConnectWizzardViewModel expectedResult = new();
        _memoryCache.Set<string>($"session_idkey", expectedResult.Encode(), new TimeSpan(0, 1, 0));

        //Act
        ConnectWizzardViewModel result = _cacheService.RetrieveConnectWizzardViewModel(default!);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void ThenRetrieveConnectWizzardViewModelWhichHasBeenEncoded()
    {
        //Arrange
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
        _memoryCache.Set<string>($"session_idkey", expectedResult.Encode(), new TimeSpan(0, 1, 0));

        //Act
        ConnectWizzardViewModel result = _cacheService.RetrieveConnectWizzardViewModel("key");

        //Assert
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
