using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingText : BaseProfessionalReferralPage
{
    private readonly TextModel _textModel;

    public WhenUsingText()
    {
        _textModel = new TextModel(ReferralDistributedCache.Object);
    }

    [Fact]
    public async Task ThenOnGetEmail()
    {
        //Act
        await _textModel.OnGetAsync("1");

        //Assert
        _textModel.TextBoxValue.Should().Be(Text);
    }

    [Theory]
    [InlineData("/ProfessionalReferral/Letter", true)]
    [InlineData("/ProfessionalReferral/ContactMethods", false)]
    public async Task ThenOnPostEmail(string expectedNextPage, bool letter)
    {
        ConnectionRequestModel.ContactMethodsSelected[(int)ConnectContactDetailsJourneyPage.Letter] = letter;

        _textModel.TextBoxValue = Telephone;

        //Act
        var result = await _textModel.OnPostAsync("1") as RedirectToPageResult;

        result.Should().NotBeNull();
        result!.PageName.Should().Be(expectedNextPage);
    }

    [Fact]
    public async Task ThenOnPostWithValidationError()
    {
        _textModel.ModelState.AddModelError("TextBoxValue", "message");

        //Act
        await _textModel.OnPostAsync("1");

        _textModel.ValidationValid.Should().BeFalse();
    }
}