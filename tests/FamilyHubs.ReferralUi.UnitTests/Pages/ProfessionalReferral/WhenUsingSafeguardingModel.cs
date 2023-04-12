using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingSafeguardingModel
{
    public void ThenOnGetSetsIdAndName()
    {
        //Arrange
        SafeguardingModel safeguardingModel = new SafeguardingModel();

        //Act
        safeguardingModel.OnGet("Id", "Name");

        //Assert
        safeguardingModel.Id.Should().Be("Id");
        safeguardingModel.Name.Should().Be("Name");

    }
}
