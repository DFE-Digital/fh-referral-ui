using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ReferralUi.UnitTests.Services;
using FamilyHubs.ServiceDirectory.Shared.Builders;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using System.Security.Policy;
using System.Security.Principal;
using System.Xml.Linq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingLocalOfferDetail
{
    [Theory]
    [InlineData(default!)]
    [InlineData("url")]
    [InlineData("https://wwww.google.com")]
    [InlineData("http://google.com")]
    public async Task ThenOnGetAsync_LocalOfferDetailWithReferralNotEnabled(string url)
    {
        //Arrange
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
        ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString());
        if (serviceDto != null && serviceDto.Contacts != null)
        {
            foreach (var linkcontact in serviceDto.Contacts.Select(linkcontact => linkcontact))
            {
                linkcontact.Url = url;
            }
        }

        if (serviceDto != null)
            mockILocalOfferClientService.Setup(x => x.GetLocalOfferById(It.IsAny<string>())).ReturnsAsync(serviceDto);

        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);
        DefaultHttpContext httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");
        httpContext.Request.Headers["Referer"] = "Referer";
        localOfferDetailModel.PageContext.HttpContext = httpContext;

        //Act 
        var result = await localOfferDetailModel.OnGetAsync("NewId", (serviceDto != null) ? serviceDto.Id.ToString() : string.Empty) as PageResult;

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PageResult>();
        if(url == null || url == "url")
        {
            localOfferDetailModel.Website.Should().BeEquivalentTo("");
        }
        else
            localOfferDetailModel.Website.Should().BeEquivalentTo(url);


    }

    [Fact]
    public async Task ThenOnGetAsync_WithNullServiceAtLocation()
    {
        //Arrange
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
        ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString());
        List<ServiceDeliveryDto> deliveryDtoList = new List<ServiceDeliveryDto>(serviceDto.ServiceDeliveries ?? new List<ServiceDeliveryDto>());
        deliveryDtoList.Add(new ServiceDeliveryDto
        {
        });
        serviceDto.ServiceDeliveries = deliveryDtoList;
        //serviceDto.ServiceAtLocations = default!;
        mockILocalOfferClientService.Setup(x => x.GetLocalOfferById(It.IsAny<string>())).ReturnsAsync(serviceDto);

        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);
        DefaultHttpContext httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");
        httpContext.Request.Headers["Referer"] = "Referer";
        localOfferDetailModel.PageContext.HttpContext = httpContext;

        //Act 
        var result = await localOfferDetailModel.OnGetAsync("NewId", serviceDto.Id.ToString()) as PageResult;

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PageResult>();
        localOfferDetailModel.Email.Should().BeNullOrEmpty();
        localOfferDetailModel.Phone.Should().BeNullOrEmpty();
        localOfferDetailModel.Website.Should().BeNullOrEmpty();

    }

    [Fact]
    public async Task ThenOnGetAsync_WithServiceAtLocationContainingInPerson()
    {
        //Arrange
        //Arrange
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
        ServiceDto serviceDto = GetTestCountyCouncilServicesDtoWithInPerson(Guid.NewGuid().ToString());
        mockILocalOfferClientService.Setup(x => x.GetLocalOfferById(It.IsAny<string>())).ReturnsAsync(serviceDto);

        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);
        DefaultHttpContext httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");
        httpContext.Request.Headers["Referer"] = "Referer";
        localOfferDetailModel.PageContext.HttpContext = httpContext;

        //Act 
        var result = await localOfferDetailModel.OnGetAsync("NewId", serviceDto.Id.ToString()) as PageResult;

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PageResult>();
        localOfferDetailModel.Phone.Should().Be("Telephone");
        localOfferDetailModel.Website.Should().Be("Url");
        localOfferDetailModel.Email.Should().Be("Email");

    }

    [Fact]
    public async Task ThenOnGetAsync_LocalOfferDetailWithReferralEnabled()
    {
        //Arrange
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("IsReferralEnabled", "true")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
        ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString());
        
        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);

        var identity = new ClaimsIdentity(); // empty claims identity will set IsAuthenticted = false
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var mockPrincipal = new Mock<IPrincipal>();
        mockPrincipal.Setup(x => x.Identity).Returns(identity);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(m => m.User).Returns(claimsPrincipal);
        localOfferDetailModel.PageContext.HttpContext = mockHttpContext.Object;


        //Act 
        var result = await localOfferDetailModel.OnGetAsync("NewId", serviceDto.Id.ToString()) as RedirectToPageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/SignIn");
    }

    [Fact]
    public void ThenOnPostAsync_ReturnsRedirectToPageResult()
    {
        //Arrange
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
        ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString());
        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);
        

        //Act 
        var result = localOfferDetailModel.OnPost("NewId", serviceDto.Id.ToString(), serviceDto.Name) as RedirectToPageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/ConnectFamilyToServiceStart");
    }

    [Fact]
    public void ThenGetDeliveryMethodsAsString_WithNullCollection()
    {
        //Act
        //Arrange
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);

        //Act
        string result = localOfferDetailModel.GetDeliveryMethodsAsString(default!);

        //Assert
        result.Should().Be(string.Empty);

    }

    [Fact]
    public void ThenGetLanguagesAsString_WithNullCollection()
    {
        //Act
        //Arrange
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);

        //Act
        string result = localOfferDetailModel.GetLanguagesAsString(default!);

        //Assert
        result.Should().Be(string.Empty);

    }

    [Fact]
    public void ThenGetLanguagesAsString_ShouldReturnLanguages()
    {
        //Arrange
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);
        List<LanguageDto> languageDtos = new List<LanguageDto>
        {
            new LanguageDto{Id = 1, Name = "English" },
             new LanguageDto{Id = 2, Name = "French" }
        };

        //Act
        string result = localOfferDetailModel.GetLanguagesAsString(languageDtos);

        //Assert
        result.Should().Be("English,French");

    }

    [Fact]
    public void ThenExtractAddressParts_ShouldJustReturn()
    {
        //Arrange
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);
        PhysicalAddressDto addressDto = new PhysicalAddressDto(Guid.NewGuid().ToString(), default!, null, default!, null, null);

        //Act
        localOfferDetailModel.ExtractAddressParts(addressDto);
        
        //Assert
        localOfferDetailModel.Address_1.Should().BeNullOrEmpty();
        localOfferDetailModel.Postal_code.Should().BeNullOrEmpty();
        
    }

    [Fact]
    public async Task ThenOnGetAsync_ReturnUrl_MustBeSetToRefererHeaderForNavigation()
    {
        //Arrange
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        Mock<ILocalOfferClientService> mockILocalOfferClientService = new Mock<ILocalOfferClientService>();
        ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString());
        mockILocalOfferClientService.Setup(x => x.GetLocalOfferById(It.IsAny<string>())).ReturnsAsync(serviceDto);

        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockILocalOfferClientService.Object, configuration);
        DefaultHttpContext httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");
        httpContext.Request.Headers["Referer"] = "postcode/1";
        localOfferDetailModel.PageContext.HttpContext = httpContext;
        var expectedReturnUrl = "postcode/1";

        //Act 
        var result = await localOfferDetailModel.OnGetAsync("NewId", serviceDto.Id) as PageResult;

        //Assert
        localOfferDetailModel.Should().NotBeNull();
        Assert.Equal(expectedReturnUrl, localOfferDetailModel?.ReturnUrl?.ToString());
    }

    public static ServiceDto GetTestCountyCouncilServicesDtoWithInPerson(string parentId)
    {
        var contactId = Guid.NewGuid().ToString();

        ServicesDtoBuilder builder = new ServicesDtoBuilder();
        ServiceDto service = builder.WithMainProperties(
                id: "3010521b-6e0a-41b0-b610-200edbbeeb14",
                serviceType: new ServiceTypeDto("1", "Information Sharing", ""),
                organisationId: parentId,
                name: "Unit Test Service",
                description: @"Unit Test Service Description",
                accreditations: null,
                assuredDate: null,
                attendingAccess: null,
                attendingType: null,
                deliverableType: null,
                status: "active",
                fees: null,
                canFamilyChooseDeliveryLocation: false)
            .WithServiceDelivery(new List<ServiceDeliveryDto>
                {
                    new ServiceDeliveryDto(Guid.NewGuid().ToString(),ServiceDeliveryType.InPerson)
                })
            .WithEligibility(new List<EligibilityDto>
                {
                    new EligibilityDto("Test9111Children","",0,13)
                })
            .WithLinkContact(new List<LinkContactDto>
            {
                new LinkContactDto("L1", "3010521b-6e0a-41b0-b610-200edbbeeb14", "Service",
                new ContactDto(
                    contactId,
                    "Contact",
                    string.Empty,
                    "01827 65777",
                    "01827 65777",
                    "url",
                    "Email"))
            })
            .WithCostOption(new List<CostOptionDto>())
            .WithLanguages(new List<LanguageDto>()
                {
                    new LanguageDto("1bb6c313-648d-4226-9e96-b7d37eaeb3dd", "English")
                })
            .WithServiceAreas(new List<ServiceAreaDto>()
                {
                    new ServiceAreaDto(Guid.NewGuid().ToString(), "National", null,"http://statistics.data.gov.uk/id/statistical-geography/K02000001")
                })
            .WithServiceAtLocations(new List<ServiceAtLocationDto>()
                {
                    new ServiceAtLocationDto(
                        "Test1749",
                        new LocationDto(
                            "6ea31a4f-7dcc-4350-9fba-20525efe092f",
                            "",
                            "",
                            52.6312,
                            -1.66526,
                            new List<PhysicalAddressDto>()
                            {
                                new PhysicalAddressDto(
                                    Guid.NewGuid().ToString(),
                                    "77 Sheepcote Lane",
                                    ", Stathe, Tamworth, Staffordshire, ",
                                    "B77 3JN",
                                    "England",
                                    null
                                    )
                            },
                            new List<LinkTaxonomyDto>(),
                            new List<LinkContactDto>()
                            {
                                new LinkContactDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Contact", new ContactDto(id: Guid.NewGuid().ToString(), title: "Title", name: "Name", telephone: "Telephone", textPhone: "Textphone", url: "Url", email: "Email"))
                            }
                            //new List<Accessibility_For_Disabilities>()
                            ),
                            new List<RegularScheduleDto>(),
                            new List<HolidayScheduleDto>(),
                            new List<LinkContactDto>()
                            {
                                new LinkContactDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Contact", new ContactDto(id: Guid.NewGuid().ToString(), title: "Title", name: "Name", telephone: "Telephone", textPhone: "Textphone", url: "Url", email: "Email"))
                            }

                        )

                })
            .WithServiceTaxonomies(new List<ServiceTaxonomyDto>()
                {
                    new ServiceTaxonomyDto
                    ("UnitTest9107",
                    new TaxonomyDto(
                        "UnitTest bccsource:Organisation",
                        "Organisation",
                        TaxonomyType.ServiceCategory,
                        null
                        )),

                    new ServiceTaxonomyDto
                    ("UnitTest9108",
                    new TaxonomyDto(
                        "UnitTest bccprimaryservicetype:38",
                        "Support",
                        TaxonomyType.ServiceCategory,
                        null
                        )),

                    new ServiceTaxonomyDto
                    ("UnitTest9109",
                    new TaxonomyDto(
                        "UnitTest bccagegroup:37",
                        "Children",
                        TaxonomyType.ServiceCategory,
                        null
                        )),

                    new ServiceTaxonomyDto
                    ("UnitTest9110",
                    new TaxonomyDto(
                        "UnitTestbccusergroup:56",
                        "Long Term Health Conditions",
                        TaxonomyType.ServiceCategory,
                        null
                        ))
                })
            .Build();

        return service;
    }
}
