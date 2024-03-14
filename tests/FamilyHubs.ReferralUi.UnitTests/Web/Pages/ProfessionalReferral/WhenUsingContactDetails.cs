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

        Assert.Equal(_contactDetailsModel.SelectedValues.Count(), ConnectionRequestModel.ContactMethodsSelected.Count(selected => selected));

        string[] selectedValues = _contactDetailsModel.SelectedValues.ToArray();

        foreach (string selectedValue in selectedValues)
        {
            Assert.True(Enum.TryParse(selectedValue, out ConnectContactDetailsJourneyPage result));
            Assert.True(ConnectionRequestModel.ContactMethodsSelected[(int)result]);
        }
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
        List<string> selectedValues = new();

        if (email) selectedValues.Add(ConnectContactDetailsJourneyPage.Email.ToString());
        if (telephone) selectedValues.Add(ConnectContactDetailsJourneyPage.Telephone.ToString());
        if (textphone) selectedValues.Add(ConnectContactDetailsJourneyPage.Textphone.ToString());
        if (letter) selectedValues.Add(ConnectContactDetailsJourneyPage.Letter.ToString());

        _contactDetailsModel.SelectedValues = selectedValues;

        //Act
        var result = await _contactDetailsModel.OnPostAsync("1") as RedirectToPageResult;

        result.Should().NotBeNull();
        result!.PageName.Should().Be(expectedNextPage);
    }
}
