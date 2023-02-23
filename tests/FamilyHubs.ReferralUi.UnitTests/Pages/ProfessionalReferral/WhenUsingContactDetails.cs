using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingContactDetails : BaseProfessionalReferralPage
{
    private readonly ContactDetailsModel _contactDetailsModel;

    public WhenUsingContactDetails()
    {
        _contactDetailsModel = new ContactDetailsModel(_mockIRedisCacheService.Object);
    }

    [Fact]
    public void ThenOnGetContactDetails()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);

        //Act
        _contactDetailsModel.OnGet();

        //Assert
        _contactDetailsModel.Email.Should().Be(_connectWizzardViewModel.EmailAddress);
        _contactDetailsModel.Telephone.Should().Be(_connectWizzardViewModel.Telephone);
        _contactDetailsModel.Textphone.Should().Be(_connectWizzardViewModel.Textphone);
        _contactDetailsModel.ContactSelection.Count.Should().Be(3);
    }

    [Fact]
    public void ThenOnPostContactDetails_WithNoContactsEntered()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);

        //Act
        _contactDetailsModel.OnPost();

        //Assert
        _contactDetailsModel.ValidationValid.Should().BeFalse();
    }

    [Fact]
    public void ThenOnPostContactDetails_WithContactsEnteredIncorrectly()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _contactDetailsModel.ContactSelection = new List<string>
        {
            "telephone"
        };
        

        //Act
        _contactDetailsModel.OnPost();

        //Assert
        _contactDetailsModel.ValidationValid.Should().BeFalse();
    }

    [Fact]
    public void ThenOnPostContactDetails_WithModelError()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _contactDetailsModel.Telephone = _connectWizzardViewModel.Telephone;
        _contactDetailsModel.ContactSelection = new List<string>
        {
            "phone"
        };
        _contactDetailsModel.ModelState.AddModelError("Test", "Test");


        //Act
        _contactDetailsModel.OnPost();

        //Assert
        _contactDetailsModel.ValidationValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("email")]
    [InlineData("telephone")]
    [InlineData("textphone")]
    public void ThenOnPostContactDetails_WithContactsEntered(string contactMethod)
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _contactDetailsModel.ContactSelection = new List<string>();
        
        switch (contactMethod)
        {
            case "email":
                _contactDetailsModel.Email = _connectWizzardViewModel.EmailAddress;
                _contactDetailsModel.ContactSelection.Add("email");
                break;

            case "telephone":
                _contactDetailsModel.Telephone = _connectWizzardViewModel.Telephone;
                _contactDetailsModel.ContactSelection.Add("telephone");
                break;

            case "textphone":
                _contactDetailsModel.Textphone = _connectWizzardViewModel.Textphone;
                _contactDetailsModel.ContactSelection.Add("textphone");
                break;

        }


        //Act
        var result = _contactDetailsModel.OnPost() as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/WhySupport");
    }

    [Theory]
    [InlineData("email", "someone")]
    [InlineData("telephone", "0121 111 2222")]
    [InlineData("textphone", "0121 111 2222")]
    [InlineData("email", default!)]
    [InlineData("telephone", default!)]
    [InlineData("textphone", default!)]
    public void ThenOnPostContactDetails_WithContactsEntered_ButIncorrect(string contactMethod, string contactEntry)
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _contactDetailsModel.ContactSelection = new List<string>();

        switch (contactMethod)
        {
            case "email":
                _contactDetailsModel.Email = contactEntry;
                _contactDetailsModel.ContactSelection.Add(contactMethod);
                break;

            case "telephone":
                _contactDetailsModel.Telephone = contactEntry;
                _contactDetailsModel.ContactSelection.Add(contactMethod);
                break;

            case "textphone":
                _contactDetailsModel.Textphone = contactEntry;
                _contactDetailsModel.ContactSelection.Add(contactMethod);
                break;

        }


        //Act
        _contactDetailsModel.OnPost();

        //Assert
        _contactDetailsModel.ValidationValid.Should().BeFalse();
    }
}
