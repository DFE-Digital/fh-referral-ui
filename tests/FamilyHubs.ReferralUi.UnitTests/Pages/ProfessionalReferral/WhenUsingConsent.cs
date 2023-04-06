using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

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
        //Act
        _consentModel.OnGet("Id","Name");

        //Assert
        _consentModel.Id.Should().Be("Id");
        _consentModel.Name.Should().Be("Name");
    }

    [Fact]
    public void ThenOnGetConsent_With_IsConsentGiven_NotSelected()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _consentModel.IsConsentGiven = default!;

        //Act
        _consentModel.OnPost("Id", "ServiceName");

        //Assert
        _consentModel.ValidationValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("yes", "/ProfessionalReferral/FamilyContact")]
    [InlineData("no", "/ProfessionalReferral/ConsentShutter")]
    public void ThenOnGetConsent_With_IsImmediateHarm_Selected(string isConsentGiven, string pageName)
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _consentModel.IsConsentGiven = isConsentGiven;

        //Act
        var result = _consentModel.OnPost("Id", "ServiceName") as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be(pageName);
    }

    [Fact]
    public void ThenOnGetConsent_When_Not_Authenticated()
    {
        //Arrange
        var mockClaimPrincipal = new Mock<ClaimsPrincipal>();
#pragma warning disable CS8601 // Possible null reference assignment.
        mockClaimPrincipal.Setup(x => x.Identity.IsAuthenticated).Returns(false);
#pragma warning restore CS8601 // Possible null reference assignment.

        _consentModel.PageContext.HttpContext = new DefaultHttpContext()
        {
            User = mockClaimPrincipal.Object
        };

        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _consentModel.IsConsentGiven = "yes";

        //Act
        var result = _consentModel.OnPost("Id", "ServiceName") as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/SignIn");
    }
}
