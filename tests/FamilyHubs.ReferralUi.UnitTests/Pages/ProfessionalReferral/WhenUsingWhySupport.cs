using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingWhySupport : BaseProfessionalReferralPage
{
    private readonly WhySupportModel _whySupportModel;

    public WhenUsingWhySupport()
    {
        _whySupportModel = new WhySupportModel(_mockIRedisCacheService.Object);
    }

    [Fact]
    public void ThenOnGetWhySupport()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);

        //Act
        _whySupportModel.OnGet();

        //Assert
        _whySupportModel.ReasonForSupport.Should().Be(_connectWizzardViewModel.ReasonForSupport);
    }

    [Theory]
    [InlineData(default!)]
    [InlineData("7BKYZlrTkbLrpJIxdweadC73o5aAJQ44mhjsh2rCqonGtv4DUHLGw952WWKZGE4eEASKYCzlKRAPf8FEP2StfrHEnbNJ7h8WbVegmzML8ZZh9tjzTpYNPMOla1hKhBXaHalXGA1FWa5ultgeBxdoB4LSBYzcIks9Uqnp2MqLFIlyeQJVifuebBwwEHzyA7bxlpj50tiIJMKs6fvq3sBaPfCg3ORlJ5oN8BhpgdUXMlIau1ZLcTwo9Eei6e5o12YKTnd2gDkingYpU9D8pnzCSAX3nFs840rBz0GuORUsFfORYr3w27K2VCY08hxxRLwXiaur6D5xSIrszo4KQKz8GHnTjyQSAoZNeKE7Ao8sy40XC5zC9lijLFOseSPCNgm9twfcwqwDTYnlldi3hY51kc5Pi1xeEDMeWBY0hWX7quwk7Yt1CftzgZLElAMnnAHkH6HHKe5BBdDIk5QtFtr8uU6sG5dzXvnCAw0qgFYDQa1HYe8ZjV3MK")]
    public void ThenOnGetWhySupport_With_ReasonForSupport_NotCorrect(string reasonForSupport)
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _whySupportModel.ReasonForSupport = reasonForSupport;
        _whySupportModel.ModelState.AddModelError("ReasonForSupport", "");

        //Act
        _whySupportModel.OnPost();

        //Assert
        _whySupportModel.ValidationValid.Should().BeFalse();
    }

    [Fact]
    public void ThenOnGetWhySupport_With_ReasonForSupport()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _whySupportModel.ReasonForSupport = _connectWizzardViewModel.ReasonForSupport;

        //Act
        var result = _whySupportModel.OnPost() as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/CheckReferralDetails");
    }
}
