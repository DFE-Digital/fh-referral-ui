using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.SharedKernel;
using FluentAssertions;
using Newtonsoft.Json;

namespace FamilyHubs.ReferralUi.UnitTests.Services;

public class WhenUsingLocalOfferClientService : BaseClientService
{
    [Fact]
    public async Task ThenGetLocalOffers()
    {
        //Arrange
        List<ServiceDto> list = new()
        {
            GetTestCountyCouncilServicesDto("56e62852-1b0b-40e5-ac97-54a67ea957dc")
        };

        PaginatedList<ServiceDto> paginatedList = new();
        paginatedList.Items.AddRange(list);
        var json = JsonConvert.SerializeObject(paginatedList);
        var mockClient = GetMockClient(json);
        LocalOfferClientService localOfferClientService = new(mockClient);

        LocalOfferFilter localOfferFilter = new()
        {
            ServiceType = "Information Sharing",
            Status = "active",
            MinimumAge = null,
            MaximumAge = null,
            GivenAge = null,
            DistrictCode = "E06000023",
            Latitude = 51.448006D,
            Longtitude = -2.559788D,
            Proximity = null,
            PageNumber = 1,
            PageSize = 99,
            Text = string.Empty,
            ServiceDeliveries = null,
            IsPaidFor = null,
            TaxonmyIds = null,
            Languages = null,
            CanFamilyChooseLocation = null
        };

        //Act
        var result = await localOfferClientService.GetLocalOffers(localOfferFilter);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(paginatedList);

    }

    [Fact]
    public async Task ThenGetLocalOfferById()
    {
        //Arrange
        var service = GetTestCountyCouncilServicesDto("56e62852-1b0b-40e5-ac97-54a67ea957dc");
        var json = JsonConvert.SerializeObject(service);
        var mockClient = GetMockClient(json);
        LocalOfferClientService localOfferClientService = new(mockClient);

        //Act
        var result = await localOfferClientService.GetLocalOfferById(service.Id);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(service);

    }

    [Fact]
    public async Task GetServicesByOrganisationId()
    {
        //Arrange
        List<ServiceDto> list = new()
        {
            GetTestCountyCouncilServicesDto("56e62852-1b0b-40e5-ac97-54a67ea957dc")
        };

        var json = JsonConvert.SerializeObject(list);
        var mockClient = GetMockClient(json);
        LocalOfferClientService localOfferClientService = new(mockClient);

        //Act
        var result = await localOfferClientService.GetServicesByOrganisationId("56e62852-1b0b-40e5-ac97-54a67ea957dc");

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(list);

    }
}
