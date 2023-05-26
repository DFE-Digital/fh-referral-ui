using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingContactMethods : BaseProfessionalReferralPage
{
    private readonly ContactMethodsModel _contactMethodsModel;

    public WhenUsingContactMethods()
    {
        _contactMethodsModel = new ContactMethodsModel(ReferralDistributedCache.Object);
    }

    [Fact]
    public async Task OnGetAsync_ServiceIdIsStored()
    {
        //Act
        await _contactMethodsModel.OnGetAsync("1");

        _contactMethodsModel.ServiceId.Should().Be("1");
    }

    [Fact]
    public async Task OnGetAsync_UserInputIsStoredInEngageReason()
    {
        //Act
        await _contactMethodsModel.OnGetAsync("1");

        _contactMethodsModel.TextAreaValue.Should().Be(EngageReason);
    }

    [Fact]
    public async Task OnPostAsync_ModelIsStoredInDistributedCache()
    {
        _contactMethodsModel.TextAreaValue = "New Engage Reason";

        //Act
        await _contactMethodsModel.OnPostAsync("1");

        //Assert
        ReferralDistributedCache.Verify(x =>
            x.SetAsync(It.IsAny<string>(), It.IsAny<ConnectionRequestModel>()), Times.Once);
        var content = await ReferralDistributedCache.Object.GetAsync(ProfessionalEmail);
        ArgumentNullException.ThrowIfNull(content);
        content.EngageReason.Should().Be(_contactMethodsModel.TextAreaValue);
        
    }

    [Fact]
    public async Task OnPostAsync_UserIsRedirectedToNextPage()
    {
        _contactMethodsModel.TextAreaValue = "New Engage Reason";

        //Act
        var result = await _contactMethodsModel.OnPostAsync("1") as RedirectToPageResult;

        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/CheckDetails");
    }

    //todo: need to test error messages on get, rather than post

    //private const string EmptyErrorMessage = "Enter how best to engage with this family";
    //private const string TooLongErrorMessage = "How the service can engage with the family must be 500 characters or less";

    //[Theory]
    //[InlineData(default, EmptyErrorMessage)]
    //[InlineData("", EmptyErrorMessage)]
    //[InlineData(" ", null)]
    //[InlineData("ABC", null)]
    //[InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", null)]
    //[InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901", TooLongErrorMessage)]
    //public async Task ThenOnPostAsync_ReasonIsValidated(string? value, string? textAreaValidationErrorMessage)
    //{
    //    _contactMethodsModel.TextAreaValue = value;

    //    //Act
    //    await _contactMethodsModel.OnPostAsync("1");

    //    _contactMethodsModel.TextAreaValidationErrorMessage.Should().Be(textAreaValidationErrorMessage);
    //}
}
