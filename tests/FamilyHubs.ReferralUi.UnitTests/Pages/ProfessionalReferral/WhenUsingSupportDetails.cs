using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
        //Arrange
        Mock<ISession> mockSession = new Mock<ISession>();
        var httpContext = new DefaultHttpContext() { Session = mockSession.Object };
        _supportDetailsModel.PageContext.HttpContext = httpContext;


        //Act
        _supportDetailsModel.OnGet("Id", "Some Name With Spaces");

        //Assert
        _supportDetailsModel.ServiceId.Should().Be("Id");
        _supportDetailsModel.ServiceName.Should().Be("Some Name With Spaces");

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
        result.PageName.Should().Be("/ProfessionalReferral/WhySupport");
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
