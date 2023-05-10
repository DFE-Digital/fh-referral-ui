using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingWhySupport : BaseProfessionalReferralPage
{
    private readonly WhySupportModel _whySupportModel;

    public WhenUsingWhySupport()
    {
        _whySupportModel = new WhySupportModel(ReferralDistributedCache.Object);
    }

    [Fact]
    public async Task ThenOnGetWhySupport()
    {
        //Act
        await _whySupportModel.OnGetAsync("1");

        _whySupportModel.ServiceId.Should().Be("1");
        _whySupportModel.TextAreaValue.Should().Be(Reason);
    }

    [Fact]
    public async Task ThenOnPostWhySupport()
    {
        _whySupportModel.TextAreaValue = "New Reason For Support";

        //Act
        var result = await _whySupportModel.OnPostAsync("1") as RedirectToPageResult;

        //todo: check new content
        ReferralDistributedCache.Verify(x =>
            x.SetAsync(It.IsAny<ConnectionRequestModel>()), Times.Once);

        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/ContactDetails");
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
    //    _whySupportModel.TextAreaValue = value;

    //    //Act
    //    await _whySupportModel.OnPostAsync("1");

    //    _whySupportModel.TextAreaValidation.Should().Be(textAreaValidation);
    //}
}
