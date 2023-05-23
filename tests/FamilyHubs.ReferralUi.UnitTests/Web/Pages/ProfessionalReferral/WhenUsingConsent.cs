using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingConsent
{
    private readonly ConsentModel _consentModel;

    public WhenUsingConsent()
    {
        _consentModel = new ConsentModel();
    }

    [Fact]
    public async Task ThenOnGetConsent()
    {
        //Act
        await _consentModel.OnGetAsync("Id");

        _consentModel.ServiceId.Should().Be("Id");
    }

    //todo: check redirect to get when consent missing
    //[Fact]
    //public async Task ThenOnGetConsent_With_IsConsentGiven_NotSelected()
    //{
    //    //Arrange
    //    _consentModel.Consent = default!;

    //    //Act
    //    await _consentModel.OnPostAsync("Id");

    //    //Assert
    //    _consentModel.ValidationValid.Should().BeFalse();
    //}

    [Theory]
    [InlineData("yes", "/ProfessionalReferral/SupportDetails")]
    [InlineData("no", "/ProfessionalReferral/ConsentShutter")]
    public async Task ThenOnGetConsent_With_IsImmediateHarm_Selected(string isConsentGiven, string pageName)
    {
        //Arrange
        _consentModel.Consent = isConsentGiven;

        //Act
        var result = await _consentModel.OnPostAsync("Id") as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be(pageName);
    }
}
