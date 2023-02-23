using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FamilyHubs.SharedKernel;
using FluentAssertions;
using Newtonsoft.Json;

namespace FamilyHubs.ReferralUi.UnitTests.Services;

public class WhenUsingOrganisationClientService : BaseClientService
{
    [Fact]
    public async Task ThenGetCategories()
    {
        //Arrange
        var taxonomies = new List<TaxonomyDto>()
        {
            new TaxonomyDto("16f3a451-e88d-4ad0-b53f-c8925d1cc9e4", "Activities, clubs and groups", TaxonomyType.ServiceCategory, null),
            new TaxonomyDto("aafa1cc3-b984-4b10-89d5-27388c5432de", "Activities", TaxonomyType.ServiceCategory, "16f3a451-e88d-4ad0-b53f-c8925d1cc9e4"),
        };

        PaginatedList<TaxonomyDto> paginatedList = new PaginatedList<TaxonomyDto>(taxonomies, taxonomies.Count, 1, 1);

        var json = JsonConvert.SerializeObject(paginatedList);
        var mockClient = GetMockClient(json);
        OrganisationClientService organisationClientService = new(mockClient);
        

        //Act
        var result = await organisationClientService.GetCategories();

        //Assert
        result.Should().NotBeNull();
        result[0].Key.Should().BeEquivalentTo(taxonomies[0]);
        result[0].Value[0].Should().BeEquivalentTo(taxonomies[1]);

    }

    [Fact]
    public async Task ThenGetListOrganisations()
    {
        //Arrange
        List<OrganisationDto> list = new()
        {
            new OrganisationDto(
                "56e62852-1b0b-40e5-ac97-54a67ea957dc",
                new(string.Empty, string.Empty, string.Empty),
                "Unit Test County Council",
                "Unit Test County Council",
                null,
                new Uri("https://www.unittest.gov.uk/").ToString(),
                "https://www.unittest.gov.uk/")
        };
        var json = JsonConvert.SerializeObject(list);
        var mockClient = GetMockClient(json);
        OrganisationClientService organisationClientService = new(mockClient);

        //Act
        var result = await organisationClientService.GetListOrganisations();

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(list);
    }

    [Fact]
    public async Task ThenGetOrganisationById()
    {
        //Arrange
        var organisation = GetTestCountyCouncilDto();
        var json = JsonConvert.SerializeObject(organisation);
        var mockClient = GetMockClient(json);
        OrganisationClientService organisationClientService = new(mockClient);

        //Act
        var result = await organisationClientService.GetOrganisationById("56e62852-1b0b-40e5-ac97-54a67ea957dc");

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(organisation);
    }

    [Fact]
    public async Task ThenCreateOrganisation()
    {
        //Arrange
        var organisation = GetTestCountyCouncilDto();
        var mockClient = GetMockClient(organisation.Id);
        OrganisationClientService organisationClientService = new(mockClient);

        //Act
        var result = await organisationClientService.CreateOrganisation(organisation);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(organisation.Id);
    }

    [Fact]
    public async Task ThenUpdatOrganisation()
    {
        //Arrange
        var organisation = GetTestCountyCouncilDto();
        var mockClient = GetMockClient(organisation.Id);
        OrganisationClientService organisationClientService = new(mockClient);

        //Act
        var result = await organisationClientService.UpdateOrganisation(organisation);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(organisation.Id);
    }
}
