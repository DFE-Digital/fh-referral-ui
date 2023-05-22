﻿using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingCheckDetails : BaseProfessionalReferralPage
{
    public CheckDetailsModel CheckDetailsModel { get; set; }

    public Mock<IOrganisationClientService> OrganisationClientService;
    public Mock<IReferralClientService> ReferralClientService;

    public WhenUsingCheckDetails()
    {
        OrganisationClientService = new Mock<IOrganisationClientService>();
        ReferralClientService = new Mock<IReferralClientService>();

        CheckDetailsModel = new CheckDetailsModel(ReferralDistributedCache.Object, OrganisationClientService.Object, ReferralClientService.Object);
    }

    [Fact]
    public async Task EmailOptionNotSelected_EmailIsRemoved()
    {
        ConnectionRequestModel.ContactMethodsSelected[(int)ConnectContactDetailsJourneyPage.Email] = false;

        await CheckDetailsModel.OnGetAsync("1");
        CheckDetailsModel.ConnectionRequestModel!.EmailAddress.Should().BeNull();
    }

    [Fact]
    public async Task TelephoneOptionNotSelected_TelephoneIsRemoved()
    {
        ConnectionRequestModel.ContactMethodsSelected[(int)ConnectContactDetailsJourneyPage.Telephone] = false;

        await CheckDetailsModel.OnGetAsync("1");
        CheckDetailsModel.ConnectionRequestModel!.TelephoneNumber.Should().BeNull();
    }

    [Fact]
    public async Task TextOptionNotSelected_TextIsRemoved()
    {
        ConnectionRequestModel.ContactMethodsSelected[(int)ConnectContactDetailsJourneyPage.Textphone] = false;

        await CheckDetailsModel.OnGetAsync("1");
        CheckDetailsModel.ConnectionRequestModel!.TextphoneNumber.Should().BeNull();
    }

    [Fact]
    public async Task LetterOptionNotSelected_AddressIsRemoved()
    {
        ConnectionRequestModel.ContactMethodsSelected[(int)ConnectContactDetailsJourneyPage.Letter] = false;

        await CheckDetailsModel.OnGetAsync("1");

        CheckDetailsModel.ConnectionRequestModel!.AddressLine1.Should().BeNull();
        CheckDetailsModel.ConnectionRequestModel!.AddressLine2.Should().BeNull();
        CheckDetailsModel.ConnectionRequestModel!.TownOrCity.Should().BeNull();
        CheckDetailsModel.ConnectionRequestModel!.County.Should().BeNull();
        CheckDetailsModel.ConnectionRequestModel!.Postcode.Should().BeNull();
    }

    [Fact]
    public async Task EmailOptionSelected_EmailIsPresent()
    {
        await CheckDetailsModel.OnGetAsync("1");
        CheckDetailsModel.ConnectionRequestModel!.EmailAddress.Should().NotBeNull();
    }

    [Fact]
    public async Task TelephoneOptionSelected_TelephoneIsPresent()
    {
        await CheckDetailsModel.OnGetAsync("1");
        CheckDetailsModel.ConnectionRequestModel!.TelephoneNumber.Should().NotBeNull();
    }

    [Fact]
    public async Task TextOptionSelected_TextIsPresent()
    {
        await CheckDetailsModel.OnGetAsync("1");
        CheckDetailsModel.ConnectionRequestModel!.TextphoneNumber.Should().NotBeNull();
    }

    [Fact]
    public async Task LetterOptionSelected_AddressIsPresent()
    {
        await CheckDetailsModel.OnGetAsync("1");

        CheckDetailsModel.ConnectionRequestModel!.AddressLine1.Should().NotBeNull();
        CheckDetailsModel.ConnectionRequestModel!.AddressLine2.Should().NotBeNull();
        CheckDetailsModel.ConnectionRequestModel!.TownOrCity.Should().NotBeNull();
        CheckDetailsModel.ConnectionRequestModel!.County.Should().NotBeNull();
        CheckDetailsModel.ConnectionRequestModel!.Postcode.Should().NotBeNull();
    }
}