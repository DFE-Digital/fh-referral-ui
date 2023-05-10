using FamilyHubs.Referral.Web.Pages.Shared;
using FluentAssertions;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingSafeguardingModel
{
    [Fact]
    public async Task ThenOnGetSetsIdAndName()
    {
        //Arrange
        var safeguardingModel = new ProfessionalReferralModel();

        //Act
        await safeguardingModel.OnGetAsync("Id");

        //Assert
        safeguardingModel.ServiceId.Should().Be("Id");
    }
}
