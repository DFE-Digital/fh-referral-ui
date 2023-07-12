using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.ReferralService.Shared.Models;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FamilyHubs.ReferralUi.UnitTests.Core.ApiClients;

public class WhenUsingOrganisationClientService
{
    [Fact]
    public async Task ThenGetCategoryList()
    {
        //Arrange
        var listTaxonomies = ClientHelper.GetTaxonomies();
        PaginatedList<TaxonomyDto> expectedPaginatedList = new PaginatedList<TaxonomyDto>(listTaxonomies, listTaxonomies.Count, 1, listTaxonomies.Count);
        var jsonString = JsonSerializer.Serialize(expectedPaginatedList);
        
        HttpClient httpClient = ClientHelper.GetMockClient<string>(jsonString);
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer token");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        IOrganisationClientService organisationClientService = new OrganisationClientService(httpClient);

        //Act
        var result = await organisationClientService.GetCategories();

        //Assert
        result.Count.Should().Be(6);
        result[0].Value.Count.Should().Be(7); 
    }

    [Fact]
    public async Task ThenGetLocalOffers()
    {
        //Arrange
        var listServices = new List<ServiceDto>() { ClientHelper.GetTestCountyCouncilServicesDto() };
        PaginatedList<ServiceDto> expectedPaginatedList = new PaginatedList<ServiceDto>(listServices, listServices.Count, 1, listServices.Count);
        var jsonString = JsonSerializer.Serialize(expectedPaginatedList);
        HttpClient httpClient = ClientHelper.GetMockClient<string>(jsonString);
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer token");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        IOrganisationClientService organisationClientService = new OrganisationClientService(httpClient);

        LocalOfferFilter filter = new LocalOfferFilter()
        { 
            Status = "active",
            ServiceDeliveries = ServiceDeliveryType.Online.ToString(),
            IsPaidFor = true,
            TaxonomyIds = "1,2",
            Languages = "English",
            CanFamilyChooseLocation = true,
            DistrictCode = "ABC"
        };


        //Act
        var result = await organisationClientService.GetLocalOffers(filter);

        //Assert
        result.Items.Count.Should().Be(1);
        result.Items[0].Should().BeEquivalentTo(expectedPaginatedList.Items[0]);
    }

    [Fact]
    public async Task ThenGetLocalOfferById()
    {
        //Arrange
        var expectedService = ClientHelper.GetTestCountyCouncilServicesDto();
        var jsonString = JsonSerializer.Serialize(expectedService);
        HttpClient httpClient = ClientHelper.GetMockClient<string>(jsonString);
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer token");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        IOrganisationClientService organisationClientService = new OrganisationClientService(httpClient);

        //Act
        var result = await organisationClientService.GetLocalOfferById(expectedService.Id.ToString());

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedService);
    }

    [Fact]
    public async Task GetOrganisationDtobyId()
    {
        //Arrange
        var expectedOrganisation = ClientHelper.GetTestCountyCouncilWithoutAnyServices();
        var jsonString = JsonSerializer.Serialize(expectedOrganisation);
        HttpClient httpClient = ClientHelper.GetMockClient<string>(jsonString);
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer token");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        IOrganisationClientService organisationClientService = new OrganisationClientService(httpClient);

        //Act
        var result = await organisationClientService.GetOrganisationDtobyIdAsync(expectedOrganisation.Id);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedOrganisation);
    }

    [Fact]
    public void ThenCreateAddAgeToUrl()
    {
        //Arrange
        string expected = @"&minimumAge=3
&maximumAge=21
&givenAge=18";
        OrganisationClientService organisationClientService = new OrganisationClientService(new HttpClient());
        StringBuilder url = new StringBuilder();


        //Act 
        organisationClientService.AddAgeToUrl(url, 3, 21, 18);
        var result = url.ToString();

        //Assert
        result.Trim().Should().Be(expected.Trim());

    }

    [Fact]
    public void ThenAddTextToUrl()
    {
        //Arrange
        string expected = "&text=Test";
        OrganisationClientService organisationClientService = new OrganisationClientService(new HttpClient());
        StringBuilder url = new StringBuilder();


        //Act 
        organisationClientService.AddTextToUrl(url,"Test");
        var result = url.ToString();

        //Assert
        result.Trim().Should().Be(expected.Trim());

    }
}
