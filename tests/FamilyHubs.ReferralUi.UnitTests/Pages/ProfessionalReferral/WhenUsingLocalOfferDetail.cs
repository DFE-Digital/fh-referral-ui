//using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
//using FamilyHubs.ReferralUi.Ui.Services.Api;
//using FamilyHubs.ReferralUi.UnitTests.Services;
//using FamilyHubs.ServiceDirectory.Shared.Builders;
//using FamilyHubs.ServiceDirectory.Shared.Dto;
//using FamilyHubs.ServiceDirectory.Shared.Enums;
//using FluentAssertions;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.Extensions.Configuration;
//using Moq;
//using System.Security.Claims;
//using System.Security.Policy;
//using System.Security.Principal;
//using System.Xml.Linq;

//namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

//public class WhenUsingLocalOfferDetail
//{
//    [Theory]
//    [InlineData(default!)]
//    [InlineData("url")]
//    [InlineData("https://wwww.google.com")]
//    [InlineData("http://google.com")]
//    public async Task ThenOnGetAsync_LocalOfferDetailWithReferralNotEnabled(string url)
//    {
//        //Arrange
//        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
//        {
//            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
//        };

//        IConfiguration configuration = new ConfigurationBuilder()
//            .AddInMemoryCollection(inMemorySettings)
//            .Build();

//        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
//        //ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString());
//        if (serviceDto != null && serviceDto.Contacts != null)
//        {
//            foreach (var linkcontact in serviceDto.Contacts.Select(linkcontact => linkcontact))
//            {
//                linkcontact.Url = url;
//            }
//        }

//        if (serviceDto != null)
//            mockILocalOfferClientService.Setup(x => x.GetLocalOfferById(It.IsAny<string>())).ReturnsAsync(serviceDto);

//        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);
//        DefaultHttpContext httpContext = new DefaultHttpContext();
//        httpContext.Request.Scheme = "http";
//        httpContext.Request.Host = new HostString("localhost");
//        httpContext.Request.Headers["Referer"] = "Referer";
//        localOfferDetailModel.PageContext.HttpContext = httpContext;

//        //Act 
//        var result = await localOfferDetailModel.OnGetAsync("NewId", (serviceDto != null) ? serviceDto.Id.ToString() : string.Empty) as PageResult;

//        //Assert
//        result.Should().NotBeNull();
//        result.Should().BeOfType<PageResult>();
//        if(url == null || url == "url")
//        {
//            localOfferDetailModel.Website.Should().BeEquivalentTo("");
//        }
//        else
//            localOfferDetailModel.Website.Should().BeEquivalentTo(url);


//    }

//    [Fact]
//    public async Task ThenOnGetAsync_WithNullServiceAtLocation()
//    {
//        //Arrange
//        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
//        {
//            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
//        };

//        IConfiguration configuration = new ConfigurationBuilder()
//            .AddInMemoryCollection(inMemorySettings)
//            .Build();

//        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
//        ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString());
//        List<ServiceDeliveryDto> deliveryDtoList = new List<ServiceDeliveryDto>(serviceDto.ServiceDeliveries ?? new List<ServiceDeliveryDto>());
//        deliveryDtoList.Add(new ServiceDeliveryDto
//        {
//        });
//        serviceDto.ServiceDeliveries = deliveryDtoList;
//        //serviceDto.ServiceAtLocations = default!;
//        mockILocalOfferClientService.Setup(x => x.GetLocalOfferById(It.IsAny<string>())).ReturnsAsync(serviceDto);

//        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);
//        DefaultHttpContext httpContext = new DefaultHttpContext();
//        httpContext.Request.Scheme = "http";
//        httpContext.Request.Host = new HostString("localhost");
//        httpContext.Request.Headers["Referer"] = "Referer";
//        localOfferDetailModel.PageContext.HttpContext = httpContext;

//        //Act 
//        var result = await localOfferDetailModel.OnGetAsync("NewId", serviceDto.Id.ToString()) as PageResult;

//        //Assert
//        result.Should().NotBeNull();
//        result.Should().BeOfType<PageResult>();
//        localOfferDetailModel.Email.Should().BeNullOrEmpty();
//        localOfferDetailModel.Phone.Should().BeNullOrEmpty();
//        localOfferDetailModel.Website.Should().BeNullOrEmpty();

//    }

//    [Fact]
//    public async Task ThenOnGetAsync_LocalOfferDetailWithReferralEnabled()
//    {
//        //Arrange
//        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
//        {
//            new KeyValuePair<string, string?>("IsReferralEnabled", "true")
//        };

//        IConfiguration configuration = new ConfigurationBuilder()
//            .AddInMemoryCollection(inMemorySettings)
//            .Build();

//        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
//        ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString());
        
//        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);

//        var identity = new ClaimsIdentity(); // empty claims identity will set IsAuthenticted = false
//        var claimsPrincipal = new ClaimsPrincipal(identity);
//        var mockPrincipal = new Mock<IPrincipal>();
//        mockPrincipal.Setup(x => x.Identity).Returns(identity);
//        var mockHttpContext = new Mock<HttpContext>();
//        mockHttpContext.Setup(m => m.User).Returns(claimsPrincipal);
//        localOfferDetailModel.PageContext.HttpContext = mockHttpContext.Object;


//        //Act 
//        var result = await localOfferDetailModel.OnGetAsync("NewId", serviceDto.Id.ToString()) as RedirectToPageResult;

//        //Assert
//        result.Should().NotBeNull();
//        ArgumentNullException.ThrowIfNull(result);
//        result.PageName.Should().Be("/ProfessionalReferral/SignIn");
//    }

//    [Fact]
//    public void ThenOnPostAsync_ReturnsRedirectToPageResult()
//    {
//        //Arrange
//        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
//        {
//            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
//        };

//        IConfiguration configuration = new ConfigurationBuilder()
//            .AddInMemoryCollection(inMemorySettings)
//            .Build();

//        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
//        ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString());
//        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);
        

//        //Act 
//        var result = localOfferDetailModel.OnPost("NewId", serviceDto.Id.ToString(), serviceDto.Name) as RedirectToPageResult;

//        //Assert
//        result.Should().NotBeNull();
//        ArgumentNullException.ThrowIfNull(result);
//        result.PageName.Should().Be("/ProfessionalReferral/ConnectFamilyToServiceStart");
//    }

//    [Fact]
//    public void ThenGetDeliveryMethodsAsString_WithNullCollection()
//    {
//        //Act
//        //Arrange
//        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
//        {
//            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
//        };

//        IConfiguration configuration = new ConfigurationBuilder()
//            .AddInMemoryCollection(inMemorySettings)
//            .Build();

//        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
//        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);

//        //Act
//        string result = localOfferDetailModel.GetDeliveryMethodsAsString(default!);

//        //Assert
//        result.Should().Be(string.Empty);

//    }

//    [Fact]
//    public void ThenGetLanguagesAsString_WithNullCollection()
//    {
//        //Act
//        //Arrange
//        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
//        {
//            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
//        };

//        IConfiguration configuration = new ConfigurationBuilder()
//            .AddInMemoryCollection(inMemorySettings)
//            .Build();

//        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
//        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);

//        //Act
//        string result = localOfferDetailModel.GetLanguagesAsString(default!);

//        //Assert
//        result.Should().Be(string.Empty);

//    }

//    [Fact]
//    public void ThenGetLanguagesAsString_ShouldReturnLanguages()
//    {
//        //Arrange
//        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
//        {
//            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
//        };

//        IConfiguration configuration = new ConfigurationBuilder()
//            .AddInMemoryCollection(inMemorySettings)
//            .Build();

//        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
//        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);
//        List<LanguageDto> languageDtos = new List<LanguageDto>
//        {
//            new LanguageDto{Id = 1, Name = "English" },
//             new LanguageDto{Id = 2, Name = "French" }
//        };

//        //Act
//        string result = localOfferDetailModel.GetLanguagesAsString(languageDtos);

//        //Assert
//        result.Should().Be("English,French");

//    }

//    [Fact]
//    public void ThenExtractAddressParts_ShouldJustReturn()
//    {
//        //Arrange
//        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
//        {
//            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
//        };

//        IConfiguration configuration = new ConfigurationBuilder()
//            .AddInMemoryCollection(inMemorySettings)
//            .Build();

//        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
//        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);
//        //Act
       
        
//        //Assert
//        localOfferDetailModel.Address_1.Should().BeNullOrEmpty();
//        localOfferDetailModel.Postal_code.Should().BeNullOrEmpty();
        
//    }

//    [Fact]
//    public async Task ThenOnGetAsync_ReturnUrl_MustBeSetToRefererHeaderForNavigation()
//    {
//        //Arrange
//        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
//        {
//            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
//        };

//        IConfiguration configuration = new ConfigurationBuilder()
//            .AddInMemoryCollection(inMemorySettings)
//            .Build();

//        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
//        //ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString());
//        mockILocalOfferClientService.Setup(x => x.GetLocalOfferById(It.IsAny<string>())).ReturnsAsync(serviceDto);

//        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);
//        DefaultHttpContext httpContext = new DefaultHttpContext();
//        httpContext.Request.Scheme = "http";
//        httpContext.Request.Host = new HostString("localhost");
//        httpContext.Request.Headers["Referer"] = "postcode/1";
//        localOfferDetailModel.PageContext.HttpContext = httpContext;
//        var expectedReturnUrl = "postcode/1";

//        //Act 
//        var result = await localOfferDetailModel.OnGetAsync("NewId", serviceDto.Id.ToString()) as PageResult;

//        //Assert
//        localOfferDetailModel.Should().NotBeNull();
//        Assert.Equal(expectedReturnUrl, localOfferDetailModel?.ReturnUrl?.ToString());
//    }

//}
