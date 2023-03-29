using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.SharedKernel;
using FluentAssertions;
using Newtonsoft.Json;

namespace FamilyHubs.ReferralUi.UnitTests.Services;

public class WhenUsingLocalOfferClientService : BaseClientService
{
    //[Theory]
    //[InlineData(null, null, null)]
    //[InlineData(51.448006D, -2.559788D, null)]
    //[InlineData(51.448006D, -2.559788D, 10.0D)]
    //public async Task ThenGetLocalOffers(double? latitude, double? longtitude, double? proximity)
    //{
    //    //Arrange
    //    List<ServiceDto> list = new()
    //    {
    //        GetTestCountyCouncilServicesDto("56e62852-1b0b-40e5-ac97-54a67ea957dc")
    //    };

    //    PaginatedList<ServiceDto> paginatedList = new();
    //    paginatedList.Items.AddRange(list);
    //    var json = JsonConvert.SerializeObject(paginatedList);
    //    var mockClient = GetMockClient(json);
    //    LocalOfferClientService localOfferClientService = new(mockClient);

    //    LocalOfferFilter localOfferFilter = new()
    //    {
    //        ServiceType = "Information Sharing",
    //        Status = default!,
    //        MinimumAge = 1,
    //        MaximumAge = 20,
    //        GivenAge = 19,
    //        DistrictCode = "E06000023",
    //        Latitude = latitude,
    //        Longtitude = longtitude,
    //        Proximity = proximity,
    //        PageNumber = 1,
    //        PageSize = 99,
    //        Text = "Some Value",
    //        ServiceDeliveries = "Service Delivery",
    //        IsPaidFor = false,
    //        TaxonmyIds = "1",
    //        Languages = "English",
    //        CanFamilyChooseLocation = true
    //    };

    //    //Act
    //    var result = await localOfferClientService.GetLocalOffers(localOfferFilter);

    //    //Assert
    //    result.Should().NotBeNull();
    //    result.Should().BeEquivalentTo(paginatedList);

    //}

    //[Fact]
    //public async Task ThenGetLocalOfferById()
    //{
    //    //Arrange
    //    var service = GetTestCountyCouncilServicesDto("56e62852-1b0b-40e5-ac97-54a67ea957dc");
    //    var json = JsonConvert.SerializeObject(service);
    //    var mockClient = GetMockClient(json);
    //    LocalOfferClientService localOfferClientService = new(mockClient);

    //    //Act
    //    var result = await localOfferClientService.GetLocalOfferById(service.Id.ToString());

    //    //Assert
    //    result.Should().NotBeNull();
    //    result.Should().BeEquivalentTo(service);

    //}

    //[Fact]
    //public async Task GetServicesByOrganisationId()
    //{
    //    //Arrange
    //    List<ServiceDto> list = new()
    //    {
    //        GetTestCountyCouncilServicesDto("56e62852-1b0b-40e5-ac97-54a67ea957dc")
    //    };

    //    var json = JsonConvert.SerializeObject(list);
    //    var mockClient = GetMockClient(json);
    //    LocalOfferClientService localOfferClientService = new(mockClient);

    //    //Act
    //    var result = await localOfferClientService.GetServicesByOrganisationId("56e62852-1b0b-40e5-ac97-54a67ea957dc");

    //    //Assert
    //    result.Should().NotBeNull();
    //    result.Should().BeEquivalentTo(list);

    //}

}
