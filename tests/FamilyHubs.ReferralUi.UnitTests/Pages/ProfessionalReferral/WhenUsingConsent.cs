using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingConsent : BaseProfessionalReferralPage
{
    private readonly ConsentModel _consentModel;

    public WhenUsingConsent()
    {
        _consentModel = new ConsentModel(_mockIRedisCacheService.Object);
    }

    [Fact]
    public void ThenOnGetConsent()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);

        //Act
        _consentModel.OnGet();

        //Assert
        _consentModel.IsConsentGiven.Should().Be("yes");
    }

    [Fact]
    public void ThenOnGetConsent_With_IsConsentGiven_NotSelected()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _consentModel.IsConsentGiven = default!;

        //Act
        _consentModel.OnPost();

        //Assert
        _consentModel.ValidationValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("yes", "/ProfessionalReferral/FamilyContact")]
    [InlineData("no", "/ProfessionalReferral/ConsentShutter")]
    public void ThenOnGetSafeguarding_With_IsImmediateHarm_Selected(string isConsentGiven, string pageName)
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _consentModel.IsConsentGiven = isConsentGiven;

        //Act
        var result = _consentModel.OnPost() as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be(pageName);
    }
}
