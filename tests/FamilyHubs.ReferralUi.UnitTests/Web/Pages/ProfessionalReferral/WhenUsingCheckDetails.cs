using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.Notifications;
using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingCheckDetails : BaseProfessionalReferralPage
{
    public CheckDetailsModel CheckDetailsModel { get; set; }

    public Mock<IOrganisationClientService> OrganisationClientService;
    public Mock<IReferralClientService> ReferralClientService;
    public Mock<INotifications> Notifications;
    public Mock<IConfiguration> Configuration;

    public WhenUsingCheckDetails()
    {
        OrganisationClientService = new Mock<IOrganisationClientService>();
        ReferralClientService = new Mock<IReferralClientService>();
        Notifications = new Mock<INotifications>();
        Configuration = new Mock<IConfiguration>();

        Configuration.Setup(x => x["RequestsSentUrl"]).Returns("https://example.com");
        Configuration.Setup(x => x["NotificationTemplateIds:VcsNewRequest"]).Returns("123");

        CheckDetailsModel = new CheckDetailsModel(
            ReferralDistributedCache.Object,
            OrganisationClientService.Object,
            ReferralClientService.Object,
            Notifications.Object,
            Configuration.Object)
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
    public async Task ThenOnPostAsync_NextPageIsConfirmation()
    {
        OrganisationClientService
            .Setup(x => x.GetLocalOfferById(It.IsAny<string>()))
            .ReturnsAsync(new ServiceDto
            {
                Id = 1,
                Name = "Test Service",
                Description = "Service Description",
                // other required properties
                ServiceOwnerReferenceId = "",
                ServiceType = ServiceType.InformationSharing
            });

        OrganisationClientService
            .Setup(x => x.GetOrganisationDtobyIdAsync(It.IsAny<long>()))
            .ReturnsAsync(new OrganisationDto
            {
                Id = 1,
                Name = "Test Organisation",
                Description = "Organisation Description",
                // other required properties
                OrganisationType = OrganisationType.VCFS,
                AdminAreaCode = ""
            });

        var result = await CheckDetailsModel.OnPostAsync("1") as RedirectToPageResult;

        result.Should().NotBeNull();
        result!.PageName.Should().Be("/ProfessionalReferral/Confirmation");
    }

    [Fact]
    public async Task OnPostAsync_ThenNotificationIsSent()
    {
        OrganisationClientService
            .Setup(x => x.GetLocalOfferById(It.IsAny<string>()))
            .ReturnsAsync(new ServiceDto
            {
                Id = 1,
                Name = "Test Service",
                Description = "Service Description",
                // other required properties
                ServiceOwnerReferenceId = "",
                ServiceType = ServiceType.InformationSharing
            });

        OrganisationClientService
            .Setup(x => x.GetOrganisationDtobyIdAsync(It.IsAny<long>()))
            .ReturnsAsync(new OrganisationDto
            {
                Id = 1,
                Name = "Test Organisation",
                Description = "Organisation Description",
                // other required properties
                OrganisationType = OrganisationType.VCFS,
                AdminAreaCode = ""
            });

        await CheckDetailsModel.OnPostAsync("1");

        Notifications.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("https://example.com/")]
    public async Task OnPostAsync_HandlesRequestSentUrlWithAndWithoutTrailingSlash(string requestSentUrl)
    {
        Configuration.Setup(x => x["RequestsSentUrl"]).Returns(requestSentUrl);

        OrganisationClientService
            .Setup(x => x.GetLocalOfferById(It.IsAny<string>()))
            .ReturnsAsync(new ServiceDto
            {
                Id = 1,
                Name = "Test Service",
                Description = "Service Description",
                // other required properties
                ServiceOwnerReferenceId = "",
                ServiceType = ServiceType.InformationSharing
            });

        OrganisationClientService
            .Setup(x => x.GetOrganisationDtobyIdAsync(It.IsAny<long>()))
            .ReturnsAsync(new OrganisationDto
            {
                Id = 1,
                Name = "Test Organisation",
                Description = "Organisation Description",
                // other required properties
                OrganisationType = OrganisationType.VCFS,
                AdminAreaCode = ""
            });

        await CheckDetailsModel.OnPostAsync("1");

        Notifications.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}