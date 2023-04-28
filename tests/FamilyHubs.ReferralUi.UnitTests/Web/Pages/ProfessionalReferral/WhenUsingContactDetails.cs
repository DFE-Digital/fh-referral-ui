﻿using FamilyHubs.Referral.Core.DistributedCache;
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
        _connectionRequestModel.TextphoneSelected = textphone;
        _connectionRequestModel.LetterSelected = letter;

        //Act
        await _contactDetailsModel.OnGetAsync("1");

        _contactDetailsModel.Email.Should().Be(email);
        _contactDetailsModel.Telephone.Should().Be(telephone);
        _contactDetailsModel.Textphone.Should().Be(textphone);
        _contactDetailsModel.Letter.Should().Be(letter);
    }

    [Theory]
    [InlineData("/ProfessionalReferral/Email", true, false, false, false)]
    [InlineData("/ProfessionalReferral/Telephone", false, true, false, false)]
    [InlineData("/ProfessionalReferral/Textphone", false, false, true, false)]
    [InlineData("/ProfessionalReferral/Letter", false, false, false, true)]
    [InlineData("/ProfessionalReferral/Email", true, true, true, true)]
    [InlineData("/ProfessionalReferral/Telephone", false, true, true, true)]
    [InlineData("/ProfessionalReferral/Textphone", false, false, true, true)]
    public async Task ThenOnPostSupportDetails(string expectedNextPage, bool email, bool telephone, bool textphone, bool letter)
    {
        _contactDetailsModel.Email = email;
        _contactDetailsModel.Telephone = telephone;
        _contactDetailsModel.Textphone = textphone;
        _contactDetailsModel.Letter = letter;

        //Act
        var result = await _contactDetailsModel.OnPostAsync() as RedirectToPageResult;

        result.Should().NotBeNull();
        result!.PageName.Should().Be(expectedNextPage);
    }

    [Fact]
    public async Task ThenOnPostWithValidationError()
    {
        //Act
        var result = await _contactDetailsModel.OnPostAsync() as RedirectToPageResult;

        //Assert
        _contactDetailsModel.ValidationValid.Should().BeFalse();
    }
}