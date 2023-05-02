using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingEmail : BaseProfessionalReferralPage
{
    private readonly EmailModel _emailModel;

    public WhenUsingEmail()
    {
        _emailModel = new EmailModel(ReferralDistributedCache.Object);
    }

    [Fact]
    public async Task ThenOnGetEmail()
    {
        //Arrange and Act
        await _emailModel.OnGetAsync("1");

        //Assert
        _emailModel.TextBoxValue.Should().Be(EmailAddress);
    }

    [Theory]
    [InlineData("/ProfessionalReferral/Telephone", true, false, false)]
    [InlineData("/ProfessionalReferral/Textphone", false, true, false)]
    [InlineData("/ProfessionalReferral/Letter", false, false, true)]
    [InlineData("/ProfessionalReferral/Telephone", true, true, true)]
    [InlineData("/ProfessionalReferral/Textphone", false, true, true)]
    [InlineData("/ProfessionalReferral/ContactMethod", false, false, false)]
    public async Task ThenOnPostEmail(string expectedNextPage, bool telephone, bool textphone, bool letter)
    {
        ConnectionRequestModel.TelephoneSelected = telephone;
        ConnectionRequestModel.TextphoneSelected = textphone;
        ConnectionRequestModel.LetterSelected = letter;

        _emailModel.TextBoxValue = "someone@email.com";

        //Act
        var result = await _emailModel.OnPostAsync() as RedirectToPageResult;

        result.Should().NotBeNull();
        result!.PageName.Should().Be(expectedNextPage);
    }

    [Fact]
    public async Task ThenOnPostWithValidationError()
    {
        _emailModel.ModelState.AddModelError("key", "message");

        //Act
        await _emailModel.OnPostAsync();

        _emailModel.ValidationValid.Should().BeFalse();
    }
}
