using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingTelephone : BaseProfessionalReferralPage
{
    private readonly TelephoneModel _telephoneModel;

    public WhenUsingTelephone()
    {
        _telephoneModel = new TelephoneModel(_mockICacheService.Object);
    }

    [Fact]
    public void ThenOnGetTelephone()
    {
        //Arrange and Act
        _telephoneModel.OnGet();

        //Assert
        _telephoneModel.TextBoxValue.Should().Be(_connectWizzardViewModel.TelephoneNumber);
    }

    [Theory]
    [InlineData(2, "/ProfessionalReferral/Textphone")]
    [InlineData(3, "/ProfessionalReferral/Letter")]
    [InlineData(4, "/ProfessionalReferral/ContactMethod")]

    public void ThenOnPostEmail(int version, string url)
    {
        //Arrange
        _telephoneModel.TextBoxValue = "0121 111 3333";
        _mockICacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(WhenUsingEmail.GetConnectWizzardViewModel(version));


        //Act
        var result = _telephoneModel.OnPost() as RedirectToPageResult;


        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be(url);
    }

    [Theory]
    [InlineData(default!)]
    [InlineData("")]
    [InlineData("0121 A")]
    public void ThenOnPostWithValidationError(string value)
    {
        //Arrange
        _telephoneModel.TextBoxValue = value;

        //Arrange and Act
        _telephoneModel.OnPost();

        //Assert
        _telephoneModel.PartialTextBoxViewModel.ValidationValid.Should().BeFalse();
    }
}
