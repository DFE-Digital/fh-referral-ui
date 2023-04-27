using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingContactDetails : BaseProfessionalReferralPage
{
    private readonly ContactDetailsModel _contactDetailsModel;
    private readonly Mock<IConnectionRequestDistributedCache> _mockConnectionRequestDistributedCache;
    private readonly ConnectionRequestModel _connectionRequestModel;

    public WhenUsingContactDetails()
    {
        _connectionRequestModel = new ConnectionRequestModel
        {
            ServiceId = "Service Id",
            ServiceName = "Service Name",
            FamilyContactFullName = "Full Name",
            Reason = "Reason for Support",
        };
        _mockConnectionRequestDistributedCache = new Mock<IConnectionRequestDistributedCache>();
        _mockConnectionRequestDistributedCache.Setup(x => x.GetAsync()).ReturnsAsync(_connectionRequestModel);

        _contactDetailsModel = new ContactDetailsModel(_mockConnectionRequestDistributedCache.Object);
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
        _connectionRequestModel.EmailSelected = email;
        _connectionRequestModel.TelephoneSelected = telephone;
        _connectionRequestModel.TextPhoneSelected = textphone;
        _connectionRequestModel.LetterSelected = letter;

        //Act
        await _contactDetailsModel.OnGetAsync("1");

        _contactDetailsModel.Email.Should().Be(email);
        _contactDetailsModel.Telephone.Should().Be(telephone);
        _contactDetailsModel.Textphone.Should().Be(textphone);
        _contactDetailsModel.Letter.Should().Be(letter);
    }

    //[Theory]
    //[InlineData("Email", "/ProfessionalReferral/Email")]
    //[InlineData("Telephone", "/ProfessionalReferral/Telephone")]
    //[InlineData("Textphone", "/ProfessionalReferral/Textphone")]
    //[InlineData("Letter", "/ProfessionalReferral/Letter")]
    //public void ThenOnPostSupportDetails(string value, string urlDesination)
    //{
    //    //Arrange
    //    switch (value)
    //    {
    //        case "Telephone":
    //            _contactDetailsModel.Telephone = "Telephone";
    //            break;
    //        case "Textphone":
    //            _contactDetailsModel.Textphone = "Textphone";
    //            break;
    //        case "Letter":
    //            _contactDetailsModel.Letter = "Letter";
    //            break;
    //        default:
    //            _contactDetailsModel.Email = "Email";
    //            break;
    //    }


    //    //Act
    //    var result = _contactDetailsModel.OnPost() as RedirectToPageResult;


    //    //Assert
    //    ArgumentNullException.ThrowIfNull(result);
    //    result.PageName.Should().Be(urlDesination);
    //}

    //[Fact]
    //public void ThenOnPostWithValidationError()
    //{
    //    //Arrange and Act
    //    var result = _contactDetailsModel.OnPost() as RedirectToPageResult;

    //    //Assert
    //    _contactDetailsModel.ValidationValid.Should().BeFalse();
    //}
}
