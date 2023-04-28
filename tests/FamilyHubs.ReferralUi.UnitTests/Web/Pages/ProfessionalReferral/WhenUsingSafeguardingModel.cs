using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingSafeguardingModel
{
    [Fact]
    public void ThenOnGetSetsIdAndName()
    {
        //Arrange
        SafeguardingModel safeguardingModel = new SafeguardingModel();

        //Act
        safeguardingModel.OnGet("Id", "Name");

        //Assert
        safeguardingModel.ServiceId.Should().Be("Id");
        safeguardingModel.ServiceName.Should().Be("Name");

    }
}
