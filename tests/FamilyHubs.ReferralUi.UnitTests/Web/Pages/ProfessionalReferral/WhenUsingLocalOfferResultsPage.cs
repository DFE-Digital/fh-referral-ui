//using System.Text.Json;
//using FamilyHubs.Referral.Core.ApiClients;
//using FamilyHubs.Referral.Core.Models;
//using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
//using FamilyHubs.ServiceDirectory.Shared.Dto;
//using FamilyHubs.ServiceDirectory.Shared.Enums;
//using FamilyHubs.ServiceDirectory.Shared.Models;
//using FamilyHubs.SharedKernel.Razor.Pagination;
//using FluentAssertions;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.Extensions.Primitives;
//using Moq;

//// ReSharper disable StringLiteralTypo

//namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

//public class WhenUsingLocalOfferResultsPage
//{
//    private readonly LocalOfferResultsModel _pageModel;
//    private readonly Mock<IPostcodeLocationClientService> _mockIPostcodeLocationClientService;
//    private readonly Mock<IOrganisationClientService> _mockIOrganisationClientService;

//    public WhenUsingLocalOfferResultsPage()
//    {
//        _mockIPostcodeLocationClientService = new Mock<IPostcodeLocationClientService>();
//        _mockIOrganisationClientService = new Mock<IOrganisationClientService>();

//        _mockIOrganisationClientService.Setup(x => x.GetCategories())
//            .ReturnsAsync(new List<KeyValuePair<TaxonomyDto, List<TaxonomyDto>>>());

//        _mockIOrganisationClientService.Setup(x => x.GetLocalOffers(It.IsAny<LocalOfferFilter>()))
//            .ReturnsAsync(new PaginatedList<ServiceDto>());

//        _pageModel = new LocalOfferResultsModel(_mockIPostcodeLocationClientService.Object, _mockIOrganisationClientService.Object);
//    }

//    [Theory]
//    [InlineData(null)]
//    [InlineData("")]
//    public async Task ThenOnGetAsync_WhenSearchPostCodeIsNullOrEmpty_ThenNoResultsShouldBeReturned(string? postCode)
//    {
//        // Act
//        var searchResults = await _pageModel.OnGetAsync(postCode, "", "", "", "", "", "", 1, false) as PageResult;

//        // Assert
//        searchResults!.Model.Should().BeNull();
//    }

//    [Fact]
//    public async Task OnGetAsync_WhenSearchPostCode()
//    {
//        //Arrange
//        var json = @"{
//        ""status"": 200,
//        ""result"": {
//            ""postcode"": ""BS2 0SP"",
//            ""quality"": 1,
//            ""eastings"": 361195,
//            ""northings"": 172262,
//            ""country"": ""England"",
//            ""nhs_ha"": ""South West"",
//            ""longitude"": -2.559788,
//            ""latitude"": 51.448006,
//            ""european_electoral_region"": ""South West"",
//            ""primary_care_trust"": ""Bristol"",
//            ""region"": ""South West"",
//            ""lsoa"": ""Bristol 056B"",
//            ""msoa"": ""Bristol 056"",
//            ""incode"": ""0SP"",
//            ""outcode"": ""BS2"",
//            ""parliamentary_constituency"": ""Bristol West"",
//            ""admin_district"": ""Bristol, City of"",
//            ""parish"": ""Bristol, City of, unparished area"",
//            ""admin_county"": null,
//            ""date_of_introduction"": ""199412"",
//            ""admin_ward"": ""Lawrence Hill"",
//            ""ced"": null,
//            ""ccg"": ""NHS Bristol, North Somerset and South Gloucestershire"",
//            ""nuts"": ""Bristol, City of"",
//            ""pfa"": ""Avon and Somerset"",
//            ""codes"": {
//                ""admin_district"": ""E06000023"",
//                ""admin_county"": ""E99999999"",
//                ""admin_ward"": ""E05010907"",
//                ""parish"": ""E43000019"",
//                ""parliamentary_constituency"": ""E14000602"",
//                ""ccg"": ""E38000222"",
//                ""ccg_id"": ""15C"",
//                ""ced"": ""E99999999"",
//                ""nuts"": ""TLK11"",
//                ""lsoa"": ""E01014658"",
//                ""msoa"": ""E02006889"",
//                ""lau2"": ""E06000023"",
//                ""pfa"": ""E23000036""
//                }
//            }
//        }";

//        var postcodesIoResponse = JsonSerializer.Deserialize<PostcodesIoResponse>(json) ?? new PostcodesIoResponse();
//        _mockIPostcodeLocationClientService.Setup(x => x.LookupPostcode(It.IsAny<string>())).ReturnsAsync(postcodesIoResponse);
//        _mockIOrganisationClientService.Setup(x => x.GetCategories()).ReturnsAsync(GetTaxonomies());
//        _mockIOrganisationClientService.Setup(x => x.GetLocalOffers(It.IsAny<LocalOfferFilter>())).ReturnsAsync(new PaginatedList<ServiceDto>());
//        // Act
//        var searchResults = await _pageModel.OnGetAsync("BS2 0SP", "1", "127", "", "", "", "", 1, false) as PageResult;

//        // Assert
//        searchResults.Should().NotBeNull();
//    }

//    [Theory]
//    [InlineData("paid")]
//    [InlineData("free")]
//    public void ThenOnPostAsync_LocalOfferResults(string costSelection)
//    {
//        //Arrange
//        var json = @"{
//    ""status"": 200,
//    ""result"": {
//        ""postcode"": ""BS2 0SP"",
//        ""quality"": 1,
//        ""eastings"": 361195,
//        ""northings"": 172262,
//        ""country"": ""England"",
//        ""nhs_ha"": ""South West"",
//        ""longitude"": -2.559788,
//        ""latitude"": 51.448006,
//        ""european_electoral_region"": ""South West"",
//        ""primary_care_trust"": ""Bristol"",
//        ""region"": ""South West"",
//        ""lsoa"": ""Bristol 056B"",
//        ""msoa"": ""Bristol 056"",
//        ""incode"": ""0SP"",
//        ""outcode"": ""BS2"",
//        ""parliamentary_constituency"": ""Bristol West"",
//        ""admin_district"": ""Bristol, City of"",
//        ""parish"": ""Bristol, City of, unparished area"",
//        ""admin_county"": null,
//        ""date_of_introduction"": ""199412"",
//        ""admin_ward"": ""Lawrence Hill"",
//        ""ced"": null,
//        ""ccg"": ""NHS Bristol, North Somerset and South Gloucestershire"",
//        ""nuts"": ""Bristol, City of"",
//        ""pfa"": ""Avon and Somerset"",
//        ""codes"": {
//            ""admin_district"": ""E06000023"",
//            ""admin_county"": ""E99999999"",
//            ""admin_ward"": ""E05010907"",
//            ""parish"": ""E43000019"",
//            ""parliamentary_constituency"": ""E14000602"",
//            ""ccg"": ""E38000222"",
//            ""ccg_id"": ""15C"",
//            ""ced"": ""E99999999"",
//            ""nuts"": ""TLK11"",
//            ""lsoa"": ""E01014658"",
//            ""msoa"": ""E02006889"",
//            ""lau2"": ""E06000023"",
//            ""pfa"": ""E23000036""
//        }
//    }
//}";

//        _pageModel.CostSelection = new List<string>
//        {
//            costSelection
//        };
//        _pageModel.ServiceDeliverySelection = new List<string>();
//        _pageModel.CategorySelection = new List<string>();
//        _pageModel.SubcategorySelection = new List<string>();

//        var httpContext = new DefaultHttpContext();
//        httpContext.Request.Scheme = "http";
//        httpContext.Request.Host = new HostString("localhost");
//        var formCol = new FormCollection(new Dictionary<string, StringValues>
//        {
//            { "SearchText", "Test" }
//        });
//        httpContext.Request.ContentType = "application/x-www-form-urlencoded";
//        httpContext.Request.Form = formCol;

//        _pageModel.PageContext.HttpContext = httpContext;

//        var postcodesIoResponse = JsonSerializer.Deserialize<PostcodesIoResponse>(json) ?? new PostcodesIoResponse();
//        _mockIPostcodeLocationClientService.Setup(x => x.LookupPostcode(It.IsAny<string>())).ReturnsAsync(postcodesIoResponse);
//        _mockIOrganisationClientService.Setup(x => x.GetCategories()).ReturnsAsync(GetTaxonomies());

//        // Act
//        var searchResults = _pageModel.OnPostAsync(
//            removeCostSelection: "yes",
//            removeFilter: true,
//            removeServiceDeliverySelection: "yes",
//            removeSelectedLanguage: "yes",
//            removeForChildrenAndYoungPeople: "yes",
//            removeSearchAge: "yes",
//            removecategorySelection: "yes",
//            removesubcategorySelection: "yes") as RedirectToPageResult;

//        // Assert
//        searchResults.Should().NotBeNull();
//        searchResults!.PageName.Should().Be("/ProfessionalReferral/LocalOfferResults");
//    }

//    [Theory]
//    [InlineData("paid")]
//    [InlineData("free")]
//    public void ThenOnPostAsync_LocalOfferResults_WithModelStateError(string costSelection)
//    {
//        //Arrange
//        var json = @"{
//    ""status"": 200,
//    ""result"": {
//        ""postcode"": ""BS2 0SP"",
//        ""quality"": 1,
//        ""eastings"": 361195,
//        ""northings"": 172262,
//        ""country"": ""England"",
//        ""nhs_ha"": ""South West"",
//        ""longitude"": -2.559788,
//        ""latitude"": 51.448006,
//        ""european_electoral_region"": ""South West"",
//        ""primary_care_trust"": ""Bristol"",
//        ""region"": ""South West"",
//        ""lsoa"": ""Bristol 056B"",
//        ""msoa"": ""Bristol 056"",
//        ""incode"": ""0SP"",
//        ""outcode"": ""BS2"",
//        ""parliamentary_constituency"": ""Bristol West"",
//        ""admin_district"": ""Bristol, City of"",
//        ""parish"": ""Bristol, City of, unparished area"",
//        ""admin_county"": null,
//        ""date_of_introduction"": ""199412"",
//        ""admin_ward"": ""Lawrence Hill"",
//        ""ced"": null,
//        ""ccg"": ""NHS Bristol, North Somerset and South Gloucestershire"",
//        ""nuts"": ""Bristol, City of"",
//        ""pfa"": ""Avon and Somerset"",
//        ""codes"": {
//            ""admin_district"": ""E06000023"",
//            ""admin_county"": ""E99999999"",
//            ""admin_ward"": ""E05010907"",
//            ""parish"": ""E43000019"",
//            ""parliamentary_constituency"": ""E14000602"",
//            ""ccg"": ""E38000222"",
//            ""ccg_id"": ""15C"",
//            ""ced"": ""E99999999"",
//            ""nuts"": ""TLK11"",
//            ""lsoa"": ""E01014658"",
//            ""msoa"": ""E02006889"",
//            ""lau2"": ""E06000023"",
//            ""pfa"": ""E23000036""
//        }
//    }
//}";

//        _pageModel.CostSelection = new List<string>
//        {
//            costSelection
//        };
//        _pageModel.ServiceDeliverySelection = new List<string>();
//        _pageModel.CategorySelection = new List<string>();
//        _pageModel.SubcategorySelection = new List<string>();
//        _pageModel.Pagination = new DontShowPagination();

//        var httpContext = new DefaultHttpContext
//        {
//            Request =
//            {
//                Scheme = "http",
//                Host = new HostString("localhost")
//            }
//        };
//        var formCol = new FormCollection(new Dictionary<string, StringValues>
//        {
//            { "SearchText", "Test" }
//        });
//        httpContext.Request.ContentType = "application/x-www-form-urlencoded";
//        httpContext.Request.Form = formCol;

//        _pageModel.PageContext.HttpContext = httpContext;

//        var postcodesIoResponse = JsonSerializer.Deserialize<PostcodesIoResponse>(json) ?? new PostcodesIoResponse();
//        _mockIPostcodeLocationClientService.Setup(x => x.LookupPostcode(It.IsAny<string>())).ReturnsAsync(postcodesIoResponse);
//        _mockIOrganisationClientService.Setup(x => x.GetCategories()).ReturnsAsync(GetTaxonomies());
//        _pageModel.ModelState.AddModelError("Error", "Has Error");

//        // Act
//        var searchResults = _pageModel.OnPostAsync(
//            removeCostSelection: "yes",
//            removeFilter: true,
//            removeServiceDeliverySelection: "yes",
//            removeSelectedLanguage: "yes",
//            removeForChildrenAndYoungPeople: "yes",
//            removeSearchAge: "yes",
//            removecategorySelection: "yes",
//            removesubcategorySelection: "yes") as RedirectToPageResult;

//        // Assert
//        searchResults.Should().NotBeNull();
//    }

//    [Fact]
//    public void ThenOnGetAddressAsString()
//    {
//        //Arrange
//        var physicalAddressDto = new LocationDto
//        {
//            LocationType = LocationType.FamilyHub,
//            Name = "Physical Address",
//            Longitude = -2.559788,
//            Latitude = 51.448006,
//            Address1 = "30 Street Name | District",
//            City = "City",
//            PostCode = "BS1 2XU",
//            Country = "United Kingdom",
//            StateProvince = "County"
//        };

//        //Act
//        var result = _pageModel.GetAddressAsString(physicalAddressDto);

//        //Assert
//        result.Should().NotBeNull();
//        result.Should().Be("30 Street Name , District,City,County,BS1 2XU");
//    }

//    [Fact]
//    public void ThenGetDeliveryMethodsAsString()
//    {
//        //Arrange
//        var serviceDeliveries = new List<ServiceDeliveryDto>
//        {
//            new ServiceDeliveryDto{  Id =1, Name = ServiceDeliveryType.Online},
//             new ServiceDeliveryDto{  Id =2, Name = ServiceDeliveryType.Telephone},
//              new ServiceDeliveryDto{  Id =3, Name = ServiceDeliveryType.InPerson}
//        };

//        //Act
//        var result = _pageModel.GetDeliveryMethodsAsString(serviceDeliveries);

//        //Assert
//        result.Should().NotBeNull();
//        result.Should().Be("Online,Telephone,In Person");
//    }

//    [Fact]
//    public void ThenGetDeliveryMethodsAsString_WithEmptyCollectionWillReturnEmptyString()
//    {
//        //Arrange
//        var serviceDeliveries = new List<ServiceDeliveryDto>();

//        //Act
//        var result = _pageModel.GetDeliveryMethodsAsString(serviceDeliveries);

//        //Assert
//        result.Should().NotBeNull();
//        result.Should().Be(string.Empty);
//    }

//    [Fact]
//    public void ThenGetLanguagesAsString()
//    {
//        //Arrange
//        var languages = new List<LanguageDto>
//        {
//            new() { Id= 1, Name = "English", Code = "en" },
//            new() { Id= 2, Name = "French", Code = "fr" },
//            new() { Id= 3, Name = "German", Code = "de" }
//        };

//        //Act
//        var result = _pageModel.GetLanguagesAsString(languages);

//        //Assert
//        result.Should().NotBeNull();
//        result.Should().Be("English,French,German");
//    }

//    [Fact]
//    public void ThenGetLanguagesAsStringWithEmptyCollectionReturnEmptyString()
//    {
//        //Arrange
//        var languages = new List<LanguageDto>();

//        //Act
//        var result = _pageModel.GetLanguagesAsString(languages);

//        //Assert
//        result.Should().NotBeNull();
//        result.Should().Be(string.Empty);
//    }

//    private List<KeyValuePair<TaxonomyDto, List<TaxonomyDto>>> GetTaxonomies()
//    {
//        var list = new List<TaxonomyDto>
//        {
//            new TaxonomyDto {Name ="Activities, clubs and groups" , TaxonomyType= TaxonomyType.ServiceCategory},
//            new TaxonomyDto { Name = "Activities", TaxonomyType = TaxonomyType.ServiceCategory },
//            new TaxonomyDto { Name = "Before and after school clubs", TaxonomyType = TaxonomyType.ServiceCategory },
//            new TaxonomyDto { Name = "Holiday clubs and schemes", TaxonomyType = TaxonomyType.ServiceCategory },
//            new TaxonomyDto { Name = "Music, arts and dance", TaxonomyType = TaxonomyType.ServiceCategory },
//            new TaxonomyDto { Name = "Parent, baby and toddler groups", TaxonomyType = TaxonomyType.ServiceCategory },
//            new TaxonomyDto { Name = "Pre-school playgroup", TaxonomyType = TaxonomyType.ServiceCategory },
//            new TaxonomyDto { Name = "Sports and recreation", TaxonomyType = TaxonomyType.ServiceCategory }
//        };

//        var keyValuePairs = new List<KeyValuePair<TaxonomyDto, List<TaxonomyDto>>>();

//        var topLevelCategories = list
//            .Where(x => x.ParentId == null && !x.Name.Contains("bccusergroupTestDelete") && x.TaxonomyType == TaxonomyType.ServiceCategory)
//            .OrderBy(x => x.Name)
//            .ToList();

//        foreach (var topLevelCategory in topLevelCategories)
//        {
//            var subCategories = list.Where(x => x.ParentId == topLevelCategory.Id).OrderBy(x => x.Name).ToList();
//            var pair = new KeyValuePair<TaxonomyDto, List<TaxonomyDto>>(topLevelCategory, subCategories);
//            keyValuePairs.Add(pair);
//        }

//        return keyValuePairs;

//    }
//}
