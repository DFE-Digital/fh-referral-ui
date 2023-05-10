using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingSupportDetails : BaseProfessionalReferralPage
{
    private readonly SupportDetailsModel _supportDetailsModel;
    public WhenUsingSupportDetails()
    {
        _supportDetailsModel = new SupportDetailsModel(ReferralDistributedCache.Object);
    }

    [Fact]
    public async Task ThenOnGetSupportDetails()
    {
        //Arrange
        Mock<ISession> mockSession = new Mock<ISession>();
        var httpContext = new DefaultHttpContext() { Session = mockSession.Object };
        _supportDetailsModel.PageContext.HttpContext = httpContext;

        //Act
        await _supportDetailsModel.OnGetAsync("Id");

        //Assert
        _supportDetailsModel.ServiceId.Should().Be("Id");
    }

    [Fact]
    public async Task ThenOnPostSupportDetails()
    {
        _supportDetailsModel.TextBoxValue = "Joe Blogs";

        //Act
        var result = await _supportDetailsModel.OnPostAsync("Id") as RedirectToPageResult;

        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/WhySupport");
    }

    [Theory]
    [InlineData(default!)]
    [InlineData(" ")]
    public async Task ThenOnPostSupportDetailsWithEmptyFullName(string value)
    {
        _supportDetailsModel.TextBoxValue = value;
        _supportDetailsModel.ModelState.AddModelError("FamilyContactFullName", "Enter a full name");

        //Act
        await _supportDetailsModel.OnPostAsync("Id");

        _supportDetailsModel.ValidationValid.Should().BeFalse();
    }
}
