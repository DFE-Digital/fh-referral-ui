using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingSafeguarding : BaseProfessionalReferralPage
{
    private readonly SafeguardingModel _safeguardingModel;

    public WhenUsingSafeguarding()
    {
        _safeguardingModel = new SafeguardingModel(_mockIRedisCacheService.Object);
    }

    [Fact]
    public void ThenOnGetSafeguarding()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);

        //Act
        _safeguardingModel.OnGet();

        //Assert
        _safeguardingModel.IsImmediateHarm.Should().Be("no");
    }

    [Fact]
    public void ThenOnGetSafeguarding_With_IsImmediateHarm_NotSelected()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _safeguardingModel.IsImmediateHarm = default!;

        //Act
        _safeguardingModel.OnPost();

        //Assert
        _safeguardingModel.ValidationValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("yes", "/ProfessionalReferral/SafeguardingShutter")]
    [InlineData("no", "/ProfessionalReferral/Consent")]
    public void ThenOnGetSafeguarding_With_IsImmediateHarm_Selected(string isImmediateHarm, string pageName)
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _safeguardingModel.IsImmediateHarm = isImmediateHarm;

        //Act
        var result = _safeguardingModel.OnPost() as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be(pageName);
    }
}
