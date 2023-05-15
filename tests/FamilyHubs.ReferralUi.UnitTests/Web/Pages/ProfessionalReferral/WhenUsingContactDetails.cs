using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingContactDetails : BaseProfessionalReferralPage
{
    private readonly ContactDetailsModel _contactDetailsModel;

    public WhenUsingContactDetails()
    {
        _contactDetailsModel = new ContactDetailsModel(ReferralDistributedCache.Object);
    }

    [Theory]
    [InlineData(false, false, false, false)]
    [InlineData(true, false, false, false)]
    [InlineData(false, true, false, false)]
    [InlineData(false, false, true, false)]
    [InlineData(false, false, false, true)]
    [InlineData(true, true, true, true)]
    public async Task ThenCheckboxesShouldMatchRetrievedModel(bool email, bool telephone, bool textphone, bool letter)
    {
        ConnectionRequestModel.ContactMethodsSelected[(int)ConnectContactDetailsJourneyPage.Email] = email;
        ConnectionRequestModel.ContactMethodsSelected[(int)ConnectContactDetailsJourneyPage.Telephone] = telephone;
        ConnectionRequestModel.ContactMethodsSelected[(int)ConnectContactDetailsJourneyPage.Textphone] = textphone;
        ConnectionRequestModel.ContactMethodsSelected[(int)ConnectContactDetailsJourneyPage.Letter] = letter;

        //Act
        await _contactDetailsModel.OnGetAsync("1");

        _contactDetailsModel.ContactMethods[(int)ConnectContactDetailsJourneyPage.Email].Should().Be(email);
        _contactDetailsModel.ContactMethods[(int)ConnectContactDetailsJourneyPage.Telephone].Should().Be(telephone);
        _contactDetailsModel.ContactMethods[(int)ConnectContactDetailsJourneyPage.Textphone].Should().Be(textphone);
        _contactDetailsModel.ContactMethods[(int)ConnectContactDetailsJourneyPage.Letter].Should().Be(letter);
    }

    [Theory]
    [InlineData("/ProfessionalReferral/Email", true, false, false, false)]
    [InlineData("/ProfessionalReferral/Telephone", false, true, false, false)]
    [InlineData("/ProfessionalReferral/Text", false, false, true, false)]
    [InlineData("/ProfessionalReferral/Letter", false, false, false, true)]
    [InlineData("/ProfessionalReferral/Email", true, true, true, true)]
    [InlineData("/ProfessionalReferral/Telephone", false, true, true, true)]
    [InlineData("/ProfessionalReferral/Text", false, false, true, true)]
    public async Task ThenOnPostSupportDetails(string expectedNextPage, bool email, bool telephone, bool textphone, bool letter)
    {
        _contactDetailsModel.ContactMethods[(int)ConnectContactDetailsJourneyPage.Email] = email;
        _contactDetailsModel.ContactMethods[(int)ConnectContactDetailsJourneyPage.Telephone] = telephone;
        _contactDetailsModel.ContactMethods[(int)ConnectContactDetailsJourneyPage.Textphone] = textphone;
        _contactDetailsModel.ContactMethods[(int)ConnectContactDetailsJourneyPage.Letter] = letter;

        //Act
        var result = await _contactDetailsModel.OnPostAsync("1") as RedirectToPageResult;

        result.Should().NotBeNull();
        result!.PageName.Should().Be(expectedNextPage);
    }

    [Fact]
    public async Task ThenOnPostWithValidationError()
    {
        //Act
        var result = await _contactDetailsModel.OnPostAsync("1") as RedirectToPageResult;

        //Assert
        _contactDetailsModel.ValidationValid.Should().BeFalse();
    }
}
