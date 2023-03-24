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
               new TaxonomyDto {Name ="Activities, clubs and groups" , TaxonomyType= ServiceDirectory.Shared.Enums.TaxonomyType.ServiceCategory},
            new TaxonomyDto { Name = "Activities", TaxonomyType = ServiceDirectory.Shared.Enums.TaxonomyType.ServiceCategory }
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
            new OrganisationDto{OrganisationType = OrganisationType.NotSet, Name= "Unit Test County Council", Description =  "Unit Test County Council", AdminAreaCode = "" }
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
        var mockClient = GetMockClient(organisation.Id.ToString());
        OrganisationClientService organisationClientService = new(mockClient);

        //Act
        var result = await organisationClientService.CreateOrganisation(organisation);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(organisation.Id.ToString());
    }

    [Fact]
    public async Task ThenUpdatOrganisation()
    {
        //Arrange
        var organisation = GetTestCountyCouncilDto();
        var mockClient = GetMockClient(organisation.Id.ToString());
        OrganisationClientService organisationClientService = new(mockClient);

        //Act
        var result = await organisationClientService.UpdateOrganisation(organisation);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(organisation.Id.ToString());
    }
}
