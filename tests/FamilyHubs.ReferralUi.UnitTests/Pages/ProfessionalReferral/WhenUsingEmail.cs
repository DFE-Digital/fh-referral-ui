using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingEmail : BaseProfessionalReferralPage
{
    private readonly EmailModel _emailModel;

    public WhenUsingEmail()
    {
        _emailModel = new EmailModel(_mockICacheService.Object);
    }

    [Fact]
    public void ThenOnGetEmail()
    {
        //Arrange and Act
        _emailModel.OnGet();

        //Assert
        _emailModel.TextBoxValue.Should().Be("joe.blogs@email.com");
    }

    [Theory]
    [InlineData(1, "/ProfessionalReferral/Telephone")]
    [InlineData(2, "/ProfessionalReferral/Textphone")]
    [InlineData(3, "/ProfessionalReferral/Letter")]
    [InlineData(4, "/ProfessionalReferral/ContactMethod")]

    public void ThenOnPostEmail(int version, string url)
    {
        //Arrange
        _emailModel.TextBoxValue = "someone@email.com";
        _mockICacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(GetConnectWizzardViewModel(version));


        //Act
        var result = _emailModel.OnPost() as RedirectToPageResult;


        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be(url);
    }

    [Fact]
    public void ThenOnPostWithValidationError()
    {
        //Arrange and Act
        _emailModel.OnPost();

        //Assert
        _emailModel.PartialTextBoxViewModel.ValidationValid.Should().BeFalse();
    }

    public static ConnectWizzardViewModel GetConnectWizzardViewModel(int version)
    {
        switch(version) 
        {
            default:
                return new ConnectWizzardViewModel
                {
                    ServiceId = "ServiceId",
                    ServiceName = "ServiceName",
                    FullName = "FullName",
                    ReasonForSupport = "Reason For Support",
                    EmailSelected = true,
                    TelephoneSelected = true,
                    TextPhoneSelected = true,
                    LetterSelected = true,
                    
                };

            case 2:
                return new ConnectWizzardViewModel
                {
                    ServiceId = "ServiceId",
                    ServiceName = "ServiceName",
                    FullName = "FullName",
                    ReasonForSupport = "Reason For Support",
                    EmailSelected = true,
                    TelephoneSelected = false,
                    TextPhoneSelected = true,
                    LetterSelected = true,
                    
                };

            case 3:
                return new ConnectWizzardViewModel
                {
                    ServiceId = "ServiceId",
                    ServiceName = "ServiceName",
                    FullName = "FullName",
                    ReasonForSupport = "Reason For Support",
                    EmailSelected = true,
                    TelephoneSelected = false,
                    TextPhoneSelected = false,
                    LetterSelected = true,
                };

                case 4:
                return new ConnectWizzardViewModel
                {
                    ServiceId = "ServiceId",
                    ServiceName = "ServiceName",
                    FullName = "FullName",
                    ReasonForSupport = "Reason For Support",
                    EmailSelected = true,
                    TelephoneSelected = false,
                    TextPhoneSelected = false,
                    LetterSelected = false,
                };
        }
    }
}
