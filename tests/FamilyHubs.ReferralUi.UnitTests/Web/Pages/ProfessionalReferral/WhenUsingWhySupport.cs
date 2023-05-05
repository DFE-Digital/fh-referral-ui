using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingWhySupport
{
    private readonly WhySupportModel _whySupportModel;
    private readonly Mock<IConnectionRequestDistributedCache> _mockConnectionRequestDistributedCache;
    private readonly ConnectionRequestModel _connectionRequestModel;
    public WhenUsingWhySupport()
    {
        _connectionRequestModel = new ConnectionRequestModel
        {
            ServiceId = "Service Id",
            ServiceName = "Service Name",
            FamilyContactFullName = "Full Name",
            Reason = "Reason for Support"
        };
        _mockConnectionRequestDistributedCache = new Mock<IConnectionRequestDistributedCache>();
        _mockConnectionRequestDistributedCache.Setup(x => x.GetAsync()).ReturnsAsync(_connectionRequestModel);

        _whySupportModel = new WhySupportModel(_mockConnectionRequestDistributedCache.Object);
    }

    [Fact]
    public async Task ThenOnGetWhySupport()
    {
        //Act
        await _whySupportModel.OnGetAsync("1", "Service Name");

        _whySupportModel.ServiceId.Should().Be("1");
        _whySupportModel.ServiceName.Should().Be("Service Name");
        _whySupportModel.TextAreaValue.Should().Be(_connectionRequestModel.Reason);
    }

    [Fact]
    public async Task ThenOnPostWhySupport()
    {
        _whySupportModel.TextAreaValue = "New Reason For Support";

        //Act
        var result = await _whySupportModel.OnPostAsync("1", "Service Name") as RedirectToPageResult;

        //todo: check new content
        _mockConnectionRequestDistributedCache
            .Verify(x => x.SetAsync(It.IsAny<ConnectionRequestModel>()), Times.Once);

        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/ContactDetails");
    }

    [Theory]
    [InlineData(default, TextAreaValidation.Empty)]
    [InlineData("", TextAreaValidation.Empty)]
    [InlineData(" ", TextAreaValidation.Valid)]
    [InlineData("ABC", TextAreaValidation.Valid)]
    [InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", TextAreaValidation.Valid)]
    [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901", TextAreaValidation.TooLong)]
    public async Task ThenOnPostAsync_ReasonIsValidated(string? value, TextAreaValidation textAreaValidation)
    {
        _whySupportModel.TextAreaValue = value;

        //Act
        await _whySupportModel.OnPostAsync("1", "Service Name");

        _whySupportModel.TextAreaValidation.Should().Be(textAreaValidation);
    }
}
