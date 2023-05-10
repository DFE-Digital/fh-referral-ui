using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingTelephone : BaseProfessionalReferralPage
{
    private readonly TelephoneModel _telephoneModel;

    public WhenUsingTelephone()
    {
        _telephoneModel = new TelephoneModel(ReferralDistributedCache.Object);
    }

    [Fact]
    public async Task ThenOnGetEmail()
    {
        //Act
        await _telephoneModel.OnGetAsync("1");

        //Assert
        _telephoneModel.TextBoxValue.Should().Be(Telephone);
    }

    [Theory]
    [InlineData("/ProfessionalReferral/Text", true, false)]
    [InlineData("/ProfessionalReferral/Letter", false, true)]
    [InlineData("/ProfessionalReferral/Text", true, true)]
    [InlineData("/ProfessionalReferral/ContactMethods", false, false)]
    public async Task ThenOnPostEmail(string expectedNextPage, bool textphone, bool letter)
    {
        ConnectionRequestModel.ContactMethodsSelected[(int)ConnectJourneyPage.Textphone] = textphone;
        ConnectionRequestModel.ContactMethodsSelected[(int)ConnectJourneyPage.Letter] = letter;

        _telephoneModel.TextBoxValue = Telephone;

        //Act
        var result = await _telephoneModel.OnPostAsync("1") as RedirectToPageResult;

        result.Should().NotBeNull();
        result!.PageName.Should().Be(expectedNextPage);
    }

    [Fact]
    public async Task ThenOnPostWithValidationError()
    {
        _telephoneModel.ModelState.AddModelError("TextBoxValue", "message");

        //Act
        await _telephoneModel.OnPostAsync("1");

        _telephoneModel.ValidationValid.Should().BeFalse();
    }

    //todo: we should really have a test to check the email validation is working
    // but I've been unable to get the model validation to trigger in the test
}
