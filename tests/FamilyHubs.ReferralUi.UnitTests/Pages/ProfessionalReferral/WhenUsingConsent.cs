using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingConsent
{
    private readonly ConsentModel _consentModel;

    public WhenUsingConsent()
    {
        _consentModel = new ConsentModel();
    }
    [Fact]
    public void ThenOnGetConsent()
    {
        //Act
        _consentModel.OnGet("Id", "Name");

        //Assert
        _consentModel.ServiceId.Should().Be("Id");
        _consentModel.ServiceName.Should().Be("Name");
    }

    [Fact]
    public void ThenOnGetConsent_With_IsConsentGiven_NotSelected()
    {
        //Arrange
        _consentModel.Consent = default!;

        //Act
        _consentModel.OnPost("Id", "ServiceName");

        //Assert
        _consentModel.ValidationValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("yes", "/ProfessionalReferral/SupportDetails")]
    [InlineData("no", "/ProfessionalReferral/ConsentShutter")]
    public void ThenOnGetConsent_With_IsImmediateHarm_Selected(string isConsentGiven, string pageName)
    {
        //Arrange
        _consentModel.Consent = isConsentGiven;

        //Act
        var result = _consentModel.OnPost("Id", "ServiceName") as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be(pageName);
    }

}
