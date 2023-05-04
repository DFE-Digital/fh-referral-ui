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
    [InlineData("/ProfessionalReferral/Textphone", true, false)]
    [InlineData("/ProfessionalReferral/Letter", false, true)]
    [InlineData("/ProfessionalReferral/Textphone", true, true)]
    [InlineData("/ProfessionalReferral/ContactMethod", false, false)]
    public async Task ThenOnPostEmail(string expectedNextPage, bool textphone, bool letter)
    {
        ConnectionRequestModel.TextphoneSelected = textphone;
        ConnectionRequestModel.LetterSelected = letter;

        _telephoneModel.TextBoxValue = Telephone;

        //Act
        var result = await _telephoneModel.OnPostAsync() as RedirectToPageResult;

        result.Should().NotBeNull();
        result!.PageName.Should().Be(expectedNextPage);
    }

    [Fact]
    public async Task ThenOnPostWithValidationError()
    {
        _telephoneModel.ModelState.AddModelError("TextBoxValue", "message");

        //Act
        await _telephoneModel.OnPostAsync();

        _telephoneModel.ValidationValid.Should().BeFalse();
    }

    //todo: we should really have a test to check the email validation is working
    // but I've been unable to get the model validation to trigger in the test
}
