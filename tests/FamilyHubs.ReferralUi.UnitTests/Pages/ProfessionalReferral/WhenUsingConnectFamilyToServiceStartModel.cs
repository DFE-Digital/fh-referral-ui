using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingConnectFamilyToServiceStartModel : BaseProfessionalReferralPage
{
    private readonly ConnectFamilyToServiceStartModel _connectFamilyToServiceStartModel;
    
    public WhenUsingConnectFamilyToServiceStartModel()
    {
        _connectFamilyToServiceStartModel = new ConnectFamilyToServiceStartModel(_mockIRedisCacheService.Object);
    }

    [Fact]
    public void ThenOnGetConnectFamilyToServiceStart_WithNullModelServiceId()
    {
        //Arrange
        _connectWizzardViewModel.ServiceId = default!;
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);

        //Act
        _connectFamilyToServiceStartModel.OnGet("ServiceId", "ServiceName");

        //Assert
        _connectFamilyToServiceStartModel.Id.Should().Be("ServiceId");
        _connectFamilyToServiceStartModel.Name.Should().Be("ServiceName");

    }

    [Fact]
    public void ThenOnGetConnectFamilyToServiceStart()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);

        //Act
        _connectFamilyToServiceStartModel.OnGet("ServiceId", "ServiceName");

        //Assert
        _connectFamilyToServiceStartModel.Id.Should().Be("ServiceId");
        _connectFamilyToServiceStartModel.Name.Should().Be("ServiceName");

    }

    [Fact]
    public void ThenOnPostConnectFamilyToServiceStart()
    {
        //Arrange & Act
        var result = _connectFamilyToServiceStartModel.OnPost("ServiceId", "ServiceName") as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/Safeguarding");
    }

}
