﻿using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ConnectionRequest;

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
        _whySupportModel = new WhySupportModel(_mockConnectionRequestDistributedCache.Object);
    }

    [Fact]
    public async Task ThenOnGetWhySupport()
    {
        _mockConnectionRequestDistributedCache.Setup(x => x.GetAsync()).ReturnsAsync(_connectionRequestModel);

        //Act
        await _whySupportModel.OnGetAsync("1");

        _whySupportModel.ServiceId.Should().Be(_connectionRequestModel.ServiceId);
        _whySupportModel.ServiceName.Should().Be(_connectionRequestModel.ServiceName);
        _whySupportModel.TextAreaValue.Should().Be(_connectionRequestModel.Reason);
    }

    [Fact]
    public async Task ThenOnPostWhySupport()
    {
        _mockConnectionRequestDistributedCache.Setup(x => x.GetAsync()).ReturnsAsync(_connectionRequestModel);
        _whySupportModel.TextAreaValue = "New Reason For Support";

        //Act
        var result = await _whySupportModel.OnPostAsync() as RedirectToPageResult;

        //todo: check new content
        _mockConnectionRequestDistributedCache
            .Verify(x => x.SetAsync(It.IsAny<ConnectionRequestModel>()), Times.Once);
            
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/ContactDetails");
    }

    [Theory]
    [InlineData(default)]
    [InlineData("")]
    public async Task ThenOnPostSupportDetailsWithEmptyFullName(string? value)
    {
        _whySupportModel.TextAreaValue = value;

        //Act
        await _whySupportModel.OnPostAsync();

        _whySupportModel.TextAreaValidation.Should().Be(TextAreaValidation.Empty);
    }
}
