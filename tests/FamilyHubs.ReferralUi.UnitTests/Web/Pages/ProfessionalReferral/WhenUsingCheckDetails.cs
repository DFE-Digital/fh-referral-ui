using System.Net;
using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FamilyHubs.ReferralService.Shared.Dto.CreateUpdate;
using FamilyHubs.ReferralService.Shared.Dto.Metrics;
using FamilyHubs.ReferralService.Shared.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingCheckDetails : BaseProfessionalReferralPage
{
    public CheckDetailsModel CheckDetailsModel { get; set; }

    public Mock<IReferralClientService> ReferralClientService;
    public Mock<IReferralNotificationService> ReferralNotificationService;
    public ReferralResponse ReferralResponse;

    public const long CreatedRequestNumber = 6789;
    public const long OrganisationId = 12345;
    public const string ServiceName = "No shoes, no shirt, and I still get service";

    public WhenUsingCheckDetails()
    {
        ReferralClientService = new Mock<IReferralClientService>();

        ReferralResponse = new ReferralResponse
        {
            Id = CreatedRequestNumber,
            OrganisationId = OrganisationId,
            ServiceName = ServiceName
        };

        ReferralClientService
            .Setup(s => s.CreateReferral(It.IsAny<CreateReferralDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReferralResponse, HttpStatusCode.NoContent));

        ReferralNotificationService = new Mock<IReferralNotificationService>();

        CheckDetailsModel = new CheckDetailsModel(
            ReferralDistributedCache.Object,
            ReferralClientService.Object,
            ReferralNotificationService.Object)
        {
            PageContext = GetPageContextWithUserClaims()
        };
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

    [Fact]
    public async Task ThenOnPostAsync_IfCreateReferralFailsUpdateMetricStillCalled()
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
        httpResponseMessage.RequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://example.com");

        ReferralClientService
            .Setup(s => s.CreateReferral(It.IsAny<CreateReferralDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ReferralClientServiceException(httpResponseMessage, ""));

        // Act
        await Assert.ThrowsAsync<ReferralClientServiceException>(async () =>
        {
            await CheckDetailsModel.OnPostAsync("1");
        });

        ReferralClientService.Verify(c => c.UpdateConnectionRequestsSentMetric(
            It.IsAny<UpdateConnectionRequestsSentMetricDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ThenOnPostAsync_NextPageIsConfirmation()
    {
        var result = await CheckDetailsModel.OnPostAsync("1") as RedirectToPageResult;

        result.Should().NotBeNull();
        result!.PageName.Should().Be("/ProfessionalReferral/Confirmation");
    }

    [Fact]
    public async Task OnPostAsync_ThenNotificationIsSent()
    {
        await CheckDetailsModel.OnPostAsync("1");

        ReferralNotificationService.Verify(x =>
                x.OnCreateReferral(
                    ProfessionalEmail,
                    OrganisationId,
                    ServiceName,
                    CreatedRequestNumber),
            Times.Once);
    }
}