using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingSafeguardingModel
{
    [Fact]
    public async Task ThenOnGetSetsIdAndName()
    {
        //Arrange
        SafeguardingModel safeguardingModel = new SafeguardingModel();

        //Act
        await safeguardingModel.OnGetAsync("Id", "Name");

        //Assert
        safeguardingModel.ServiceId.Should().Be("Id");
        safeguardingModel.ServiceName.Should().Be("Name");

    }
}
