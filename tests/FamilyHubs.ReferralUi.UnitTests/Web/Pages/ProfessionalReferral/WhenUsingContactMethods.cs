using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
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

    //todo: split unit test into 2
    [Fact]
    public async Task OnPostAsync_ModelIsStoredInDistributedCache()
    {
        _contactMethodsModel.TextAreaValue = "New Engage Reason";

        //Act
        var result = await _contactMethodsModel.OnPostAsync("1") as RedirectToPageResult;

        //todo: check new content
        ReferralDistributedCache.Verify(x =>
            x.SetAsync(It.IsAny<ConnectionRequestModel>()), Times.Once);
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

    //todo:
    //[Theory]
    //[InlineData(default, TextAreaValidation.Empty)]
    //[InlineData("", TextAreaValidation.Empty)]
    //[InlineData(" ", TextAreaValidation.Valid)]
    //[InlineData("ABC", TextAreaValidation.Valid)]
    //[InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", TextAreaValidation.Valid)]
    //[InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901", TextAreaValidation.TooLong)]
    //public async Task ThenOnPostAsync_ReasonIsValidated(string? value, TextAreaValidation textAreaValidation)
    //{
    //    _contactMethodsModel.TextAreaValue = value;

    //    //Act
    //    await _contactMethodsModel.OnPostAsync("1");

    //    _contactMethodsModel.TextAreaValidation.Should().Be(textAreaValidation);
    //}
}
