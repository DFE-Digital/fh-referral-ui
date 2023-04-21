using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FamilyHubs.ServiceDirectory.Shared.Models;
using FluentAssertions;
using Newtonsoft.Json;

namespace FamilyHubs.ReferralUi.UnitTests.Services;

public class WhenUsingOrganisationClientService : BaseClientService
{
    [Fact]
    public async Task ThenGetCategories()
    {
        //Arrange
        var taxonomies = new List<TaxonomyDto>
        {
            new TaxonomyDto { Id = 1, Name = "Activities, clubs and groups", TaxonomyType = TaxonomyType.ServiceCategory },
            new TaxonomyDto { Name = "Activities", TaxonomyType = TaxonomyType.ServiceCategory, ParentId = 1 }
        };

        var paginatedList = new PaginatedList<TaxonomyDto>(taxonomies, taxonomies.Count, 1, 1);

        var json = JsonConvert.SerializeObject(paginatedList);
        var mockClient = GetMockClient(json);
        var organisationClientService = new OrganisationClientService(mockClient);

        //Act
        var result = await organisationClientService.GetCategories();

        //Assert
        result.Should().NotBeNull();
        result[0].Key.Should().BeEquivalentTo(taxonomies[0]);
        result[0].Value[0].Should().BeEquivalentTo(taxonomies[1]);
    }
}