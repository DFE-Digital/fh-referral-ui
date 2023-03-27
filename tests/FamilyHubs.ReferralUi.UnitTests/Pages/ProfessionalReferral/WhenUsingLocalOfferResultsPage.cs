using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ReferralUi.UnitTests.Services;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text.Json;
using FamilyHubs.SharedKernel;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingLocalOfferResultsPage
{
    private readonly LocalOfferResultsModel _pageModel;
    private readonly Mock<IPostcodeLocationClientService> _mockIPostcodeLocationClientService;
    private readonly Mock<IOrganisationClientService> _mockIOrganisationClientService;
    private readonly Mock<ILocalOfferClientService> _mockLocalOfferClientService;

    public WhenUsingLocalOfferResultsPage()
    {
        _mockLocalOfferClientService = new Mock<ILocalOfferClientService>();
        _mockIPostcodeLocationClientService = new Mock<IPostcodeLocationClientService>();
        _mockIOrganisationClientService = new Mock<IOrganisationClientService>();

        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _pageModel = new LocalOfferResultsModel(_mockLocalOfferClientService.Object, _mockIPostcodeLocationClientService.Object, _mockIOrganisationClientService.Object, configuration);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ThenOnGetAsync_WhenSearchPostCodeIsNullOrEmpty_ThenNoResultsShouldBeReturned(string postCode)
    {
        // Act
        var searchResults = _pageModel.OnGetAsync(postCode, 0.0D, 0.0D, 0.0D, "", "", "") as IActionResult;

        // Assert
        searchResults.Should().BeNull();
    }

    [Fact]
    public async Task OnGetAsync_WhenSearchPostCode()
    {
        //Arrange
        string json = @"{
    ""status"": 200,
    ""result"": {
        ""postcode"": ""BS2 0SP"",
        ""quality"": 1,
        ""eastings"": 361195,
        ""northings"": 172262,
        ""country"": ""England"",
        ""nhs_ha"": ""South West"",
        ""longitude"": -2.559788,
        ""latitude"": 51.448006,
        ""european_electoral_region"": ""South West"",
        ""primary_care_trust"": ""Bristol"",
        ""region"": ""South West"",
        ""lsoa"": ""Bristol 056B"",
        ""msoa"": ""Bristol 056"",
        ""incode"": ""0SP"",
        ""outcode"": ""BS2"",
        ""parliamentary_constituency"": ""Bristol West"",
        ""admin_district"": ""Bristol, City of"",
        ""parish"": ""Bristol, City of, unparished area"",
        ""admin_county"": null,
        ""date_of_introduction"": ""199412"",
        ""admin_ward"": ""Lawrence Hill"",
        ""ced"": null,
        ""ccg"": ""NHS Bristol, North Somerset and South Gloucestershire"",
        ""nuts"": ""Bristol, City of"",
        ""pfa"": ""Avon and Somerset"",
        ""codes"": {
            ""admin_district"": ""E06000023"",
            ""admin_county"": ""E99999999"",
            ""admin_ward"": ""E05010907"",
            ""parish"": ""E43000019"",
            ""parliamentary_constituency"": ""E14000602"",
            ""ccg"": ""E38000222"",
            ""ccg_id"": ""15C"",
            ""ced"": ""E99999999"",
            ""nuts"": ""TLK11"",
            ""lsoa"": ""E01014658"",
            ""msoa"": ""E02006889"",
            ""lau2"": ""E06000023"",
            ""pfa"": ""E23000036""
        }
    }
}";

        PostcodesIoResponse postcodesIoResponse = JsonSerializer.Deserialize<PostcodesIoResponse>(json) ?? new PostcodesIoResponse();
        _mockIPostcodeLocationClientService.Setup(x => x.LookupPostcode(It.IsAny<string>())).ReturnsAsync(postcodesIoResponse);
        _mockIOrganisationClientService.Setup(x => x.GetCategories()).ReturnsAsync(GetTaxonomies());
        _mockLocalOfferClientService.Setup(x => x.GetLocalOffers(It.IsAny<LocalOfferFilter>())).ReturnsAsync(new PaginatedList<ServiceDto>());
        // Act
        var searchResults = await _pageModel.OnGetAsync("BS2 0SP", -2.559788D, 51.448006D, 20.0D, "1", "127", "") as Microsoft.AspNetCore.Mvc.RazorPages.PageResult;

        // Assert
        searchResults.Should().NotBeNull();
    }

    [Theory]
    [InlineData("paid")]
    [InlineData("free")]
    public async Task ThenOnPostAsync_LocalOfferResults(string costSelection)
    {
        //Arrange
        string json = @"{
    ""status"": 200,
    ""result"": {
        ""postcode"": ""BS2 0SP"",
        ""quality"": 1,
        ""eastings"": 361195,
        ""northings"": 172262,
        ""country"": ""England"",
        ""nhs_ha"": ""South West"",
        ""longitude"": -2.559788,
        ""latitude"": 51.448006,
        ""european_electoral_region"": ""South West"",
        ""primary_care_trust"": ""Bristol"",
        ""region"": ""South West"",
        ""lsoa"": ""Bristol 056B"",
        ""msoa"": ""Bristol 056"",
        ""incode"": ""0SP"",
        ""outcode"": ""BS2"",
        ""parliamentary_constituency"": ""Bristol West"",
        ""admin_district"": ""Bristol, City of"",
        ""parish"": ""Bristol, City of, unparished area"",
        ""admin_county"": null,
        ""date_of_introduction"": ""199412"",
        ""admin_ward"": ""Lawrence Hill"",
        ""ced"": null,
        ""ccg"": ""NHS Bristol, North Somerset and South Gloucestershire"",
        ""nuts"": ""Bristol, City of"",
        ""pfa"": ""Avon and Somerset"",
        ""codes"": {
            ""admin_district"": ""E06000023"",
            ""admin_county"": ""E99999999"",
            ""admin_ward"": ""E05010907"",
            ""parish"": ""E43000019"",
            ""parliamentary_constituency"": ""E14000602"",
            ""ccg"": ""E38000222"",
            ""ccg_id"": ""15C"",
            ""ced"": ""E99999999"",
            ""nuts"": ""TLK11"",
            ""lsoa"": ""E01014658"",
            ""msoa"": ""E02006889"",
            ""lau2"": ""E06000023"",
            ""pfa"": ""E23000036""
        }
    }
}";

        _pageModel.CostSelection = new List<string>()
        {
            costSelection
        };
        _pageModel.ServiceDeliverySelection = new List<string>();
        _pageModel.CategorySelection = new List<string>();
        _pageModel.SubcategorySelection = new List<string>();

        DefaultHttpContext httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");
        var formCol = new FormCollection(new Dictionary<string,
        Microsoft.Extensions.Primitives.StringValues>
        {
                    { "SearchText", "Test" }
        });
        httpContext.Request.ContentType = "application/x-www-form-urlencoded";
        httpContext.Request.Form = formCol;

        _pageModel.PageContext.HttpContext = httpContext;

        PostcodesIoResponse postcodesIoResponse = JsonSerializer.Deserialize<PostcodesIoResponse>(json) ?? new PostcodesIoResponse();
        _mockIPostcodeLocationClientService.Setup(x => x.LookupPostcode(It.IsAny<string>())).ReturnsAsync(postcodesIoResponse);
        _mockIOrganisationClientService.Setup(x => x.GetCategories()).ReturnsAsync(GetTaxonomies());
        _mockLocalOfferClientService.Setup(x => x.GetLocalOffers(It.IsAny<LocalOfferFilter>())).ReturnsAsync(new PaginatedList<ServiceDto>());

        // Act
        var searchResults = await _pageModel.OnPostAsync(removeCostSelection: "yes", 
            removeFilter: true, 
            removeServiceDeliverySelection: "yes",
            removeSelectedLanguage: "yes", 
            removeSearchAge: "yes", 
            removecategorySelection: "yes",
            removesubcategorySelection: "yes") as Microsoft.AspNetCore.Mvc.RazorPages.PageResult;

        // Assert
        searchResults.Should().NotBeNull();
    }

    [Theory]
    [InlineData("paid")]
    [InlineData("free")]
    public async Task ThenOnPostAsync_LocalOfferResults_WithModelStateError(string costSelection)
    {
        //Arrange
        string json = @"{
    ""status"": 200,
    ""result"": {
        ""postcode"": ""BS2 0SP"",
        ""quality"": 1,
        ""eastings"": 361195,
        ""northings"": 172262,
        ""country"": ""England"",
        ""nhs_ha"": ""South West"",
        ""longitude"": -2.559788,
        ""latitude"": 51.448006,
        ""european_electoral_region"": ""South West"",
        ""primary_care_trust"": ""Bristol"",
        ""region"": ""South West"",
        ""lsoa"": ""Bristol 056B"",
        ""msoa"": ""Bristol 056"",
        ""incode"": ""0SP"",
        ""outcode"": ""BS2"",
        ""parliamentary_constituency"": ""Bristol West"",
        ""admin_district"": ""Bristol, City of"",
        ""parish"": ""Bristol, City of, unparished area"",
        ""admin_county"": null,
        ""date_of_introduction"": ""199412"",
        ""admin_ward"": ""Lawrence Hill"",
        ""ced"": null,
        ""ccg"": ""NHS Bristol, North Somerset and South Gloucestershire"",
        ""nuts"": ""Bristol, City of"",
        ""pfa"": ""Avon and Somerset"",
        ""codes"": {
            ""admin_district"": ""E06000023"",
            ""admin_county"": ""E99999999"",
            ""admin_ward"": ""E05010907"",
            ""parish"": ""E43000019"",
            ""parliamentary_constituency"": ""E14000602"",
            ""ccg"": ""E38000222"",
            ""ccg_id"": ""15C"",
            ""ced"": ""E99999999"",
            ""nuts"": ""TLK11"",
            ""lsoa"": ""E01014658"",
            ""msoa"": ""E02006889"",
            ""lau2"": ""E06000023"",
            ""pfa"": ""E23000036""
        }
    }
}";

        _pageModel.CostSelection = new List<string>()
        {
            costSelection
        };
        _pageModel.ServiceDeliverySelection = new List<string>();
        _pageModel.CategorySelection = new List<string>();
        _pageModel.SubcategorySelection = new List<string>();
        _pageModel.Pagination = new DontShowPagination();

        DefaultHttpContext httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");
        var formCol = new FormCollection(new Dictionary<string,
        Microsoft.Extensions.Primitives.StringValues>
        {
                    { "SearchText", "Test" }
        });
        httpContext.Request.ContentType = "application/x-www-form-urlencoded";
        httpContext.Request.Form = formCol;

        _pageModel.PageContext.HttpContext = httpContext;

        PostcodesIoResponse postcodesIoResponse = JsonSerializer.Deserialize<PostcodesIoResponse>(json) ?? new PostcodesIoResponse();
        _mockIPostcodeLocationClientService.Setup(x => x.LookupPostcode(It.IsAny<string>())).ReturnsAsync(postcodesIoResponse);
        _mockIOrganisationClientService.Setup(x => x.GetCategories()).ReturnsAsync(GetTaxonomies());
        _pageModel.ModelState.AddModelError("Error", "Has Error");

        // Act
        var searchResults = await _pageModel.OnPostAsync(removeCostSelection: "yes",
            removeFilter: true,
            removeServiceDeliverySelection: "yes",
            removeSelectedLanguage: "yes",
            removeSearchAge: "yes",
            removecategorySelection: "yes",
            removesubcategorySelection: "yes") as Microsoft.AspNetCore.Mvc.RazorPages.PageResult;

        // Assert
        searchResults.Should().NotBeNull();
    }

    [Fact]
    public void ThenOnGetAddressAsString()
    {
        //Arrange
        var physicalAddressDto = new LocationDto
        {
            LocationType = LocationType.FamilyHub,
            Name = "Physical Address",
            Longitude = -2.559788,
            Latitude =  51.448006,
            Address1 = "30 Street Name | District",
            City =  "City",
            PostCode = "BS1 2XU",
            Country = "United Kingdom",
            StateProvince = "County"
        };

        //Act
        var result = _pageModel.GetAddressAsString(physicalAddressDto);

        //Assert
        result.Should().NotBeNull();
        result.Should().Be("30 Street Name , District,City,County,BS1 2XU");
    }

    [Fact]
    public void ThenGetDeliveryMethodsAsString()
    {
        //Arrange
        List<ServiceDeliveryDto> serviceDeliveries = new()
        {
            new ServiceDeliveryDto{  Id =1, Name = ServiceDeliveryType.Online},
             new ServiceDeliveryDto{  Id =2, Name = ServiceDeliveryType.Telephone},
              new ServiceDeliveryDto{  Id =3, Name = ServiceDeliveryType.InPerson}
        };

        //Act
        var result = _pageModel.GetDeliveryMethodsAsString(serviceDeliveries);

        //Assert
        result.Should().NotBeNull();
        result.Should().Be("Online,Telephone,In Person");
    }

    [Fact]
    public void ThenGetDeliveryMethodsAsString_WithEmptyCollectionWillReturnEmptyString()
    {
        //Arrange
        List<ServiceDeliveryDto> serviceDeliveries = new();

        //Act
        var result = _pageModel.GetDeliveryMethodsAsString(serviceDeliveries);

        //Assert
        result.Should().NotBeNull();
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ThenGetLanguagesAsString()
    {
        //Arrange
        List<LanguageDto> languages = new()
        {
            new LanguageDto{ Id= 1, Name = "English"},
             new LanguageDto{ Id= 2, Name = "French"},
              new LanguageDto{ Id= 3, Name = "German"}
        };

        //Act
        var result = _pageModel.GetLanguagesAsString(languages);

        //Assert
        result.Should().NotBeNull();
        result.Should().Be("English,French,German");
    }

    [Fact]
    public void ThenGetLanguagesAsStringWithEmptyCollectionReturnEmptyString()
    {
        //Arrange
        List<LanguageDto> languages = new();

        //Act
        var result = _pageModel.GetLanguagesAsString(languages);

        //Assert
        result.Should().NotBeNull();
        result.Should().Be(string.Empty);
    }

    [Fact]
    public async Task ThenClearFilters()
    {
        string json = @"{
    ""status"": 200,
    ""result"": {
        ""postcode"": ""BS2 0SP"",
        ""quality"": 1,
        ""eastings"": 361195,
        ""northings"": 172262,
        ""country"": ""England"",
        ""nhs_ha"": ""South West"",
        ""longitude"": -2.559788,
        ""latitude"": 51.448006,
        ""european_electoral_region"": ""South West"",
        ""primary_care_trust"": ""Bristol"",
        ""region"": ""South West"",
        ""lsoa"": ""Bristol 056B"",
        ""msoa"": ""Bristol 056"",
        ""incode"": ""0SP"",
        ""outcode"": ""BS2"",
        ""parliamentary_constituency"": ""Bristol West"",
        ""admin_district"": ""Bristol, City of"",
        ""parish"": ""Bristol, City of, unparished area"",
        ""admin_county"": null,
        ""date_of_introduction"": ""199412"",
        ""admin_ward"": ""Lawrence Hill"",
        ""ced"": null,
        ""ccg"": ""NHS Bristol, North Somerset and South Gloucestershire"",
        ""nuts"": ""Bristol, City of"",
        ""pfa"": ""Avon and Somerset"",
        ""codes"": {
            ""admin_district"": ""E06000023"",
            ""admin_county"": ""E99999999"",
            ""admin_ward"": ""E05010907"",
            ""parish"": ""E43000019"",
            ""parliamentary_constituency"": ""E14000602"",
            ""ccg"": ""E38000222"",
            ""ccg_id"": ""15C"",
            ""ced"": ""E99999999"",
            ""nuts"": ""TLK11"",
            ""lsoa"": ""E01014658"",
            ""msoa"": ""E02006889"",
            ""lau2"": ""E06000023"",
            ""pfa"": ""E23000036""
        }
    }
}";

        PostcodesIoResponse postcodesIoResponse = JsonSerializer.Deserialize<PostcodesIoResponse>(json) ?? new PostcodesIoResponse();
        _mockIPostcodeLocationClientService.Setup(x => x.LookupPostcode(It.IsAny<string>())).ReturnsAsync(postcodesIoResponse);
        _mockIOrganisationClientService.Setup(x => x.GetCategories()).ReturnsAsync(GetTaxonomies());
        _mockLocalOfferClientService.Setup(x => x.GetLocalOffers(It.IsAny<LocalOfferFilter>())).ReturnsAsync(new PaginatedList<ServiceDto>());

        // Act
        await _pageModel.ClearFilters();

        // Assert
        _pageModel.SearchResults.Items.Count.Should().Be(0);
    }

    [Theory]
    [InlineData(false, "Showing 1 search result for:")]
    [InlineData(true, "Showing 2 search results for:")]
    public void ThenGetSearchResultSnippet(bool addExtraService, string expectedResult)
    {
        //Arrange
        var list = new List<ServiceDto>()
        {
            BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString())
        };

        
        if (addExtraService)
        {
            list.Add(BaseClientService.GetTestCountyCouncilServicesDto(Guid.NewGuid().ToString()));
        }
        _pageModel.SearchResults = new SharedKernel.PaginatedList<ServiceDto>( list, list.Count, 1, 1 );

        //Act
        var result = _pageModel.SearchResultsSnippet;
        
        //Assert
        result.Should().Be(expectedResult);
    }

    private List<KeyValuePair<TaxonomyDto, List<TaxonomyDto>>> GetTaxonomies()
    {
        var list = new List<TaxonomyDto>()
        {
            new TaxonomyDto {Name ="Activities, clubs and groups" , TaxonomyType= ServiceDirectory.Shared.Enums.TaxonomyType.ServiceCategory},
            new TaxonomyDto { Name = "Activities", TaxonomyType = ServiceDirectory.Shared.Enums.TaxonomyType.ServiceCategory },
             new TaxonomyDto { Name = "Before and after school clubs", TaxonomyType = ServiceDirectory.Shared.Enums.TaxonomyType.ServiceCategory },
              new TaxonomyDto { Name = "Holiday clubs and schemes", TaxonomyType = ServiceDirectory.Shared.Enums.TaxonomyType.ServiceCategory },
              new TaxonomyDto { Name = "Music, arts and dance", TaxonomyType = ServiceDirectory.Shared.Enums.TaxonomyType.ServiceCategory },
               new TaxonomyDto { Name = "Parent, baby and toddler groups", TaxonomyType = ServiceDirectory.Shared.Enums.TaxonomyType.ServiceCategory },
               new TaxonomyDto { Name = "Pre-school playgroup", TaxonomyType = ServiceDirectory.Shared.Enums.TaxonomyType.ServiceCategory },
               new TaxonomyDto { Name = "Sports and recreation", TaxonomyType = ServiceDirectory.Shared.Enums.TaxonomyType.ServiceCategory }
        };

        List<KeyValuePair<TaxonomyDto, List<TaxonomyDto>>> keyValuePairs = new();

        var topLevelCategories = list
            .Where(x => x.ParentId == null && !x.Name.Contains("bccusergroupTestDelete") && x.TaxonomyType == TaxonomyType.ServiceCategory)
            .OrderBy(x => x.Name)
            .ToList();

        foreach (var topLevelCategory in topLevelCategories)
        {
            var subCategories = list.Where(x => x.ParentId == topLevelCategory.Id).OrderBy(x => x.Name).ToList();
            var pair = new KeyValuePair<TaxonomyDto, List<TaxonomyDto>>(topLevelCategory, subCategories);
            keyValuePairs.Add(pair);
        }

        return keyValuePairs;

    }
}
