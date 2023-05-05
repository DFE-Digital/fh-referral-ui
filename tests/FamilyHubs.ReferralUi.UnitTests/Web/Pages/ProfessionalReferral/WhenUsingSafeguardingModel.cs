using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FamilyHubs.Referral.Web.Pages.Shared;
using FluentAssertions;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingSafeguardingModel
{
    [Fact]
    public async Task ThenOnGetSetsIdAndName()
    {
        //Arrange
        var safeguardingModel = new ProfessionalReferralNoSessionModel();

        //Act
        await safeguardingModel.OnGetAsync("Id", "Name");

        //Assert
        safeguardingModel.ServiceId.Should().Be("Id");
        safeguardingModel.ServiceName.Should().Be("Name");

    }
}
