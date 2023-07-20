using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingSharePrivacy : BaseProfessionalReferralPage
{
    private readonly SharePrivacyModel _sharePrivacyModel;

    public WhenUsingSharePrivacy()
    {
        _sharePrivacyModel = new SharePrivacyModel(ReferralDistributedCache.Object);
    }

    [Fact]
    public async Task OnGet()
    {
        //Act
        await _sharePrivacyModel.OnGetAsync("Id");

        _sharePrivacyModel.ServiceId.Should().Be("Id");
    }

    [Fact]
    public async Task OnGetWithNoSelection_RedirectToSelf()
    {
        //Arrange
        _sharePrivacyModel.SharedPrivacy = null;

        //Act
        var result = await _sharePrivacyModel.OnPostAsync("Id") as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/SharePrivacy");
    }

    [Theory]
    [InlineData(true, "/ProfessionalReferral/Consent")]
    [InlineData(false, "/ProfessionalReferral/SharePrivacyShutter")]
    public async Task OnGetWithSelection_CorrectNextPage(bool sharePrivacy, string pageName)
    {
        //Arrange
        _sharePrivacyModel.SharedPrivacy = sharePrivacy;

        //Act
        var result = await _sharePrivacyModel.OnPostAsync("Id") as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be(pageName);
    }
}