using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingFamilyContact : BaseProfessionalReferralPage
{
    private readonly FamilyContactModel _familyContactModel;

    public WhenUsingFamilyContact()
    {
        _familyContactModel = new FamilyContactModel(_mockIRedisCacheService.Object);
    }

    [Fact]
    public void ThenOnGetFamilyContact()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);

        //Act
        _familyContactModel.OnGet();

        //Assert
        _familyContactModel.FullName.Should().Be(_connectWizzardViewModel.FullName);
    }

    [Theory]
    [InlineData(default!)]
    [InlineData("XbX2EHSFEmfJeE4Vf085WN9ZS5FrO20Uge16L2Z7TpPo3z6KwX9OPIMRtH9ROJHJDq9vNo2zQWY5DoupM3MzO5itlhxRcgi6RyLHOLRBafcddD7FR0sgFdfP07OYbGyPAVYhDGEidktEdWGKtUzZ2YMnW2ShF3FUJK91pZQbf90VzkkdpvtmFAd6VLQxsJDQ9nuZK9DrPRIuY8jSPMuKxWNgV3x4po07pfGNLAJH1CtboI8Zk0Ir5mSoIEvM6xRo")]
    public void ThenOnGetFamilyContact_With_FullName_NotCorrect(string fullName)
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _familyContactModel.FullName = fullName;
        _familyContactModel.ModelState.AddModelError("FullName", "");

        //Act
        _familyContactModel.OnPost();

        //Assert
        _familyContactModel.ValidationValid.Should().BeFalse();
    }

    [Fact]
    public void ThenOnGetFamilyContact_With_FullName()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        
        //Act
        var result = _familyContactModel.OnPost() as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/ContactDetails");
    }
}
