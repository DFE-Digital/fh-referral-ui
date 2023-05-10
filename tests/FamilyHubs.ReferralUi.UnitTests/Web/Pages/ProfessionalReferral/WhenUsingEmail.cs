using FamilyHubs.Referral.Core.Models;
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
        // Act
        await _emailModel.OnGetAsync("1");

        _emailModel.TextBoxValue.Should().Be(EmailAddress);
    }

    [Theory]
    [InlineData("/ProfessionalReferral/Telephone", true, false, false)]
    [InlineData("/ProfessionalReferral/Text", false, true, false)]
    [InlineData("/ProfessionalReferral/Letter", false, false, true)]
    [InlineData("/ProfessionalReferral/Telephone", true, true, true)]
    [InlineData("/ProfessionalReferral/Text", false, true, true)]
    [InlineData("/ProfessionalReferral/ContactMethods", false, false, false)]
    public async Task ThenOnPostEmail(string expectedNextPage, bool telephone, bool textphone, bool letter)
    {
        ConnectionRequestModel.ContactMethodsSelected[(int)ContactMethod.Telephone] = telephone;
        ConnectionRequestModel.ContactMethodsSelected[(int)ContactMethod.Textphone] = textphone;
        ConnectionRequestModel.ContactMethodsSelected[(int)ContactMethod.Letter] = letter;

        _emailModel.TextBoxValue = "someone@email.com";

        //Act
        var result = await _emailModel.OnPostAsync("1") as RedirectToPageResult;

        result.Should().NotBeNull();
        result!.PageName.Should().Be(expectedNextPage);
    }

    [Fact]
    public async Task ThenOnPostWithValidationError()
    {
        _emailModel.ModelState.AddModelError("key", "message");

        //Act
        await _emailModel.OnPostAsync("1");

        _emailModel.ValidationValid.Should().BeFalse();
    }

    //todo: we should really have a test to check the email validation is working
    // but I've been unable to get the model validation to trigger in the test

    //[Fact]
    //public async Task ThenOnPostWithValidationErrorX()
    //{
    //    _emailModel.TextBoxValue = "123456789012345678901234567890123456789012345678901234567890@12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890.com";

    //    //trigger model validation

    //    //Act
    //    await _emailModel.OnPostAsync();

    //    _emailModel.ValidationValid.Should().BeFalse();
    //}
}
