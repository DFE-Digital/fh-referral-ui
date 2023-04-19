using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingContactDetails : BaseProfessionalReferralPage
{
    private readonly ContactDetailsModel _contactDetailsModel;

    public WhenUsingContactDetails()
    {
        _contactDetailsModel = new ContactDetailsModel(_mockICacheService.Object);
    }

    [Fact]
    public void ThenOnGetSupportDetails()
    {
        //Arrange and Act
        _contactDetailsModel.OnGet();

        //Assert
        _contactDetailsModel.Email.Should().Be("Email");
        _contactDetailsModel.Telephone.Should().Be("Telephone");
        _contactDetailsModel.Textphone.Should().Be("Textphone");
        _contactDetailsModel.Letter.Should().Be("Letter");

    }

    [Theory]
    [InlineData("Email", "/ProfessionalReferral/Email")]
    [InlineData("Telephone", "/ProfessionalReferral/Telephone")]
    [InlineData("Textphone", "/ProfessionalReferral/Textphone")]
    [InlineData("Letter", "/ProfessionalReferral/Letter")]
    public void ThenOnPostSupportDetails(string value, string urlDesination)
    {
        //Arrange
        switch(value)
        {
            case "Telephone":
                _contactDetailsModel.Telephone = "Telephone";
            break;
            case "Textphone":
                _contactDetailsModel.Textphone = "Textphone";
                break;
            case "Letter":
                _contactDetailsModel.Letter = "Letter";
                break;
            default:
                _contactDetailsModel.Email = "Email";
                break;
        }
        

        //Act
        var result = _contactDetailsModel.OnPost() as RedirectToPageResult;


        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be(urlDesination);
    }

    [Fact]
    public void ThenOnPostWithValidationError() 
    {
        //Arrange and Act
        var result = _contactDetailsModel.OnPost() as RedirectToPageResult;

        //Assert
        _contactDetailsModel.ValidationValid.Should().BeFalse();
    }
}
