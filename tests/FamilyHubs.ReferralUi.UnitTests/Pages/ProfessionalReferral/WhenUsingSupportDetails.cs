using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingSupportDetails : BaseProfessionalReferralPage
{
    private readonly SupportDetailsModel _supportDetailsModel;
    public WhenUsingSupportDetails()
    {
        _supportDetailsModel = new SupportDetailsModel(_mockICacheService.Object);
    }

    [Fact]
    public void ThenOnGetSupportDetails()
    {
        //Arrange & Act
        _supportDetailsModel.OnGet("Id", "Some Name With Spaces");

        //Assert
        _supportDetailsModel.BackUrl.Should().Be("/ProfessionalReferral/Consent?id=Id&name=Some%20Name%20With%20Spaces");
        
    }

    [Fact]
    public void ThenOnPostSupportDetails()
    {
        //Arrange
        _supportDetailsModel.TextBoxValue = "Joe Blogs";

        //Act
        var result = _supportDetailsModel.OnPost() as RedirectToPageResult;


        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/ContactDetails");
    }

    [Theory]
    [InlineData(default!)]
    [InlineData(" ")]
    public void ThenOnPostSupportDetailsWithEmptyFullName(string value)
    {
        //Arrange
        _supportDetailsModel.TextBoxValue = value;
        _supportDetailsModel.ModelState.AddModelError("FullName", "Enter a full name");

        //Act
        _supportDetailsModel.OnPost();


        //Assert
        _supportDetailsModel.PartialTextBoxViewModel.ValidationValid.Should().BeFalse();
    }
}
