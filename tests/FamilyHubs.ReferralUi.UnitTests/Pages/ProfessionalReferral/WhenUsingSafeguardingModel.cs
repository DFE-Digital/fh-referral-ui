using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

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
        safeguardingModel.ServiceId.Should().Be("Id");
        safeguardingModel.ServiceName.Should().Be("Name");

    }
}
