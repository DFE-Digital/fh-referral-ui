﻿using FamilyHubs.Referral.Core.Models;
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

    private const string EmptyErrorMessage = "Enter a reason for the connection request";
    private const string TooLongErrorMessage = "Reason for the connection request must be 500 characters or less";

    [Theory]
    [InlineData(default, EmptyErrorMessage)]
    [InlineData("", EmptyErrorMessage)]
    [InlineData(" ", null)]
    [InlineData("ABC", null)]
    [InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", null)]
    [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901", TooLongErrorMessage)]
    public async Task ThenOnPostAsync_ReasonIsValidated(string? value, string? textAreaValidationErrorMessage)
    {
        _whySupportModel.TextAreaValue = value;

        //Act
        await _whySupportModel.OnPostAsync("1");

        _whySupportModel.TextAreaValidationErrorMessage.Should().Be(textAreaValidationErrorMessage);
    }
}
