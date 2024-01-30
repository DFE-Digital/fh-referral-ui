using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

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
        //Act
        await _supportDetailsModel.OnGetAsync("Id");

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
    [InlineData(default)]
    [InlineData(" ")]
    public async Task ThenOnPostSupportDetailsWithEmptyFullName(string? value)
    {
        _supportDetailsModel.TextBoxValue = value;
        _supportDetailsModel.ModelState.AddModelError("FamilyContactFullName", "Enter a full name");

        //Act
        var result = await _supportDetailsModel.OnPostAsync("Id") as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/SupportDetails");

        
    }
}
