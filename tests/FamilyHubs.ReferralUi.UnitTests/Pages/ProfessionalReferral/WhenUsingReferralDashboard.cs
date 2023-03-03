using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.SharedKernel;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingReferralDashboard
{
    [Fact]
    public async Task ThenOnGetReferralDashboardAsVCSAdmin()
    {
        //Arrange
        Mock<IReferralClientService> mockReferralClientService = new Mock<IReferralClientService>();
        ReferralDashboardModel referralDashboardModel = new ReferralDashboardModel(mockReferralClientService.Object);
        ReferralDto referralDto = new ReferralDto(
        id: "b7acf4db-87dd-4c7b-8295-39112d52e79a",
        organisationId: "442e1d35-5563-455a-bbd4-b3432be1508f",
        serviceId: "a8ea6d74-0619-4573-b0a6-d4cca995b331",
        serviceName: "Service Name",
        serviceDescription: "Service Description",
        serviceAsJson: "serviceAsJson",
        referrer: "referrer",
        fullName: "fullName",
        hasSpecialNeeds: "Has Special Needs",
        email: "someone@email.com",
        phone: "01211112222",
        text: "01211112222",
        dateRecieved: DateTime.UtcNow,
        requestNumber: 1L,
        reasonForSupport: "Reason For Support",
        reasonForRejection: "Reason for Rejection",
        new List<ReferralStatusDto>()
        );

        PaginatedList<ReferralDto> paginatedList = new PaginatedList<ReferralDto>(items: new List<ReferralDto> { referralDto }, 1, 1, 2);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "VCS Admin"),
            new Claim(ClaimTypes.Role, "VCSAdmin"),
            new Claim("OpenReferralOrganisationId", referralDto.OrganisationId),
        }, "mock"));
        referralDashboardModel.PageContext.HttpContext = new DefaultHttpContext() { User = user };
        mockReferralClientService.Setup(x => x.GetReferralsByOrganisationId(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool?>())).ReturnsAsync(paginatedList);

        //Act
        await referralDashboardModel.OnGetAsync(default!);

        //Assert
        referralDashboardModel.ReferralList.Should().BeEquivalentTo(paginatedList);
    }

    [Fact]
    public async Task ThenOnGetReferralDashboardAsProfessional()
    {
        //Arrange
        Mock<IReferralClientService> mockReferralClientService = new Mock<IReferralClientService>();
        ReferralDashboardModel referralDashboardModel = new ReferralDashboardModel(mockReferralClientService.Object);
        ReferralDto referralDto = new ReferralDto(
        id: "b7acf4db-87dd-4c7b-8295-39112d52e79a",
        organisationId: "442e1d35-5563-455a-bbd4-b3432be1508f",
        serviceId: "a8ea6d74-0619-4573-b0a6-d4cca995b331",
        serviceName: "Service Name",
        serviceDescription: "Service Description",
        serviceAsJson: "serviceAsJson",
        referrer: "referrer",
        fullName: "fullName",
        hasSpecialNeeds: "Has Special Needs",
        email: "someone@email.com",
        phone: "01211112222",
        text: "01211112222",
        dateRecieved: DateTime.UtcNow,
        requestNumber: 1L,
        reasonForSupport: "Reason For Support",
        reasonForRejection: "Reason for Rejection",
        new List<ReferralStatusDto>()
        );

        PaginatedList<ReferralDto> paginatedList = new PaginatedList<ReferralDto>(items: new List<ReferralDto> { referralDto }, 1, 1, 2);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "Professional"),
            new Claim(ClaimTypes.Role, "Professional"),
            new Claim("OpenReferralOrganisationId", referralDto.OrganisationId),
        }, "mock"));
        referralDashboardModel.PageContext.HttpContext = new DefaultHttpContext() { User = user };
        mockReferralClientService.Setup(x => x.GetReferralsByReferrer(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool?>())).ReturnsAsync(paginatedList);

        //Act
        await referralDashboardModel.OnGetAsync(default!);

        //Assert
        referralDashboardModel.ReferralList.Should().BeEquivalentTo(paginatedList);
    }

    [Theory]
    [InlineData("ascending")]
    [InlineData("descending")]
    public async Task ThenOnPostReferralDashboardAsVCSAdminWithDateRecievedSort(string dateRecievedDirection)
    {
        //Arrange
        Mock<IReferralClientService> mockReferralClientService = new Mock<IReferralClientService>();
        ReferralDashboardModel referralDashboardModel = new ReferralDashboardModel(mockReferralClientService.Object);
        ReferralDto referralDto = new ReferralDto(
        id: "b7acf4db-87dd-4c7b-8295-39112d52e79a",
        organisationId: "442e1d35-5563-455a-bbd4-b3432be1508f",
        serviceId: "a8ea6d74-0619-4573-b0a6-d4cca995b331",
        serviceName: "Service Name",
        serviceDescription: "Service Description",
        serviceAsJson: "serviceAsJson",
        referrer: "referrer",
        fullName: "fullName",
        hasSpecialNeeds: "Has Special Needs",
        email: "someone@email.com",
        phone: "01211112222",
        text: "01211112222",
        dateRecieved: DateTime.SpecifyKind(new DateTime(2023, 1, 2), DateTimeKind.Utc),
        requestNumber: 1L,
        reasonForSupport: "Reason For Support",
        reasonForRejection: "Reason for Rejection",
        new List<ReferralStatusDto>()
        );

        ReferralDto referralDto2 = new ReferralDto(
        id: "9efb3595-557d-4905-a330-a9a21a8ec23b",
        organisationId: "442e1d35-5563-455a-bbd4-b3432be1508f",
        serviceId: "a8ea6d74-0619-4573-b0a6-d4cca995b331",
        serviceName: "Service Name 1",
        serviceDescription: "Service Description 1",
        serviceAsJson: "serviceAsJson 1",
        referrer: "referrer",
        fullName: "fullName 1",
        hasSpecialNeeds: "Has Special Needs 1",
        email: "someone1@email.com",
        phone: "01211112223",
        text: "01211112223",
        dateRecieved: DateTime.SpecifyKind(new DateTime(2023, 1, 1), DateTimeKind.Utc),
        requestNumber: 1L,
        reasonForSupport: "Reason For Support",
        reasonForRejection: "Reason for Rejection",
        new List<ReferralStatusDto>()
        );

        PaginatedList<ReferralDto> paginatedList = new PaginatedList<ReferralDto>(items: new List<ReferralDto> { referralDto, referralDto2 }, 1, 1, 2);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "VCS Admin"),
            new Claim(ClaimTypes.Role, "VCSAdmin"),
            new Claim("OpenReferralOrganisationId", referralDto.OrganisationId),
        }, "mock"));
        referralDashboardModel.PageContext.HttpContext = new DefaultHttpContext() { User = user };
        mockReferralClientService.Setup(x => x.GetReferralsByOrganisationId(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool?>())).ReturnsAsync(paginatedList);

        //Act
        await referralDashboardModel.OnPostAsync(true, dateRecievedDirection, false, default!);

        //Assert
        if (dateRecievedDirection == "ascending")
            referralDashboardModel.ReferralList.Items[0].Should().BeEquivalentTo(referralDto);
        else
            referralDashboardModel.ReferralList.Items[0].Should().BeEquivalentTo(referralDto2);
    }

    [Theory]
    [InlineData("ascending")]
    [InlineData("descending")]
    public async Task ThenOnPostReferralDashboardAsVCSAdminWithStatusSort(string statusDirection)
    {
        //Arrange
        Mock<IReferralClientService> mockReferralClientService = new Mock<IReferralClientService>();
        ReferralDashboardModel referralDashboardModel = new ReferralDashboardModel(mockReferralClientService.Object);
        ReferralDto referralDto = new ReferralDto(
        id: "b7acf4db-87dd-4c7b-8295-39112d52e79a",
        organisationId: "442e1d35-5563-455a-bbd4-b3432be1508f",
        serviceId: "a8ea6d74-0619-4573-b0a6-d4cca995b331",
        serviceName: "Service Name",
        serviceDescription: "Service Description",
        serviceAsJson: "serviceAsJson",
        referrer: "referrer",
        fullName: "fullName",
        hasSpecialNeeds: "Has Special Needs",
        email: "someone@email.com",
        phone: "01211112222",
        text: "01211112222",
        dateRecieved: DateTime.SpecifyKind(new DateTime(2023, 1, 2), DateTimeKind.Utc),
        requestNumber: 1L,
        reasonForSupport: "Reason For Support",
        reasonForRejection: "Reason for Rejection",
        new List<ReferralStatusDto>()
        {
            new ReferralStatusDto("9efb3595-557d-4905-a330-a9a21a8ec23b", "Accept Connection")
        }
        );

        ReferralDto referralDto2 = new ReferralDto(
        id: "9efb3595-557d-4905-a330-a9a21a8ec23b",
        organisationId: "442e1d35-5563-455a-bbd4-b3432be1508f",
        serviceId: "a8ea6d74-0619-4573-b0a6-d4cca995b331",
        serviceName: "Service Name 1",
        serviceDescription: "Service Description 1",
        serviceAsJson: "serviceAsJson 1",
        referrer: "referrer",
        fullName: "fullName 1",
        hasSpecialNeeds: "Has Special Needs 1",
        email: "someone1@email.com",
        phone: "01211112223",
        text: "01211112223",
        dateRecieved: DateTime.SpecifyKind(new DateTime(2023, 1, 1), DateTimeKind.Utc),
        requestNumber: 1L,
        reasonForSupport: "Reason For Support",
        reasonForRejection: "Reason for Rejection",
        new List<ReferralStatusDto>()
        {
            new ReferralStatusDto("9efb3595-557d-4905-a330-a9a21a8ec23b", "Connection Made")
        }
        );

        PaginatedList<ReferralDto> paginatedList = new PaginatedList<ReferralDto>(items: new List<ReferralDto> { referralDto, referralDto2 }, 1, 1, 2);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "VCS Admin"),
            new Claim(ClaimTypes.Role, "VCSAdmin"),
            new Claim("OpenReferralOrganisationId", referralDto.OrganisationId),
        }, "mock"));
        referralDashboardModel.PageContext.HttpContext = new DefaultHttpContext() { User = user };
        mockReferralClientService.Setup(x => x.GetReferralsByOrganisationId(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool?>())).ReturnsAsync(paginatedList);

        //Act
        await referralDashboardModel.OnPostAsync(false, default!, true, statusDirection);

        //Assert
        if (statusDirection == "ascending")
            referralDashboardModel.ReferralList.Items[0].Should().BeEquivalentTo(referralDto2);
        else
            referralDashboardModel.ReferralList.Items[0].Should().BeEquivalentTo(referralDto);
    }

    ///
    [Theory]
    [InlineData("ascending")]
    [InlineData("descending")]
    public async Task ThenOnPostReferralDashboardAsProfessionalWithDateRecievedSort(string dateRecievedDirection)
    {
        //Arrange
        Mock<IReferralClientService> mockReferralClientService = new Mock<IReferralClientService>();
        ReferralDashboardModel referralDashboardModel = new ReferralDashboardModel(mockReferralClientService.Object);
        ReferralDto referralDto = new ReferralDto(
        id: "b7acf4db-87dd-4c7b-8295-39112d52e79a",
        organisationId: "442e1d35-5563-455a-bbd4-b3432be1508f",
        serviceId: "a8ea6d74-0619-4573-b0a6-d4cca995b331",
        serviceName: "Service Name",
        serviceDescription: "Service Description",
        serviceAsJson: "serviceAsJson",
        referrer: "referrer",
        fullName: "fullName",
        hasSpecialNeeds: "Has Special Needs",
        email: "someone@email.com",
        phone: "01211112222",
        text: "01211112222",
        dateRecieved: DateTime.SpecifyKind(new DateTime(2023, 1, 2), DateTimeKind.Utc),
        requestNumber: 1L,
        reasonForSupport: "Reason For Support",
        reasonForRejection: "Reason for Rejection",
        new List<ReferralStatusDto>()
        );

        ReferralDto referralDto2 = new ReferralDto(
        id: "9efb3595-557d-4905-a330-a9a21a8ec23b",
        organisationId: "442e1d35-5563-455a-bbd4-b3432be1508f",
        serviceId: "a8ea6d74-0619-4573-b0a6-d4cca995b331",
        serviceName: "Service Name 1",
        serviceDescription: "Service Description 1",
        serviceAsJson: "serviceAsJson 1",
        referrer: "referrer",
        fullName: "fullName 1",
        hasSpecialNeeds: "Has Special Needs 1",
        email: "someone1@email.com",
        phone: "01211112223",
        text: "01211112223",
        dateRecieved: DateTime.SpecifyKind(new DateTime(2023, 1, 1), DateTimeKind.Utc),
        requestNumber: 1L,
        reasonForSupport: "Reason For Support",
        reasonForRejection: "Reason for Rejection",
        new List<ReferralStatusDto>()
        );

        PaginatedList<ReferralDto> paginatedList = new PaginatedList<ReferralDto>(items: new List<ReferralDto> { referralDto, referralDto2 }, 1, 1, 2);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "Professional"),
            new Claim(ClaimTypes.Role, "Professional"),
            new Claim("OpenReferralOrganisationId", referralDto.OrganisationId),
        }, "mock"));
        referralDashboardModel.PageContext.HttpContext = new DefaultHttpContext() { User = user };
        mockReferralClientService.Setup(x => x.GetReferralsByReferrer(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool?>())).ReturnsAsync(paginatedList);

        //Act
        await referralDashboardModel.OnPostAsync(true, dateRecievedDirection, false, default!);

        //Assert
        if (dateRecievedDirection == "ascending")
            referralDashboardModel.ReferralList.Items[0].Should().BeEquivalentTo(referralDto);
        else
            referralDashboardModel.ReferralList.Items[0].Should().BeEquivalentTo(referralDto2);
    }

    [Theory]
    [InlineData("ascending")]
    [InlineData("descending")]
    public async Task ThenOnPostReferralDashboardAsProfessionalWithStatusSort(string statusDirection)
    {
        //Arrange
        Mock<IReferralClientService> mockReferralClientService = new Mock<IReferralClientService>();
        ReferralDashboardModel referralDashboardModel = new ReferralDashboardModel(mockReferralClientService.Object);
        ReferralDto referralDto = new ReferralDto(
        id: "b7acf4db-87dd-4c7b-8295-39112d52e79a",
        organisationId: "442e1d35-5563-455a-bbd4-b3432be1508f",
        serviceId: "a8ea6d74-0619-4573-b0a6-d4cca995b331",
        serviceName: "Service Name",
        serviceDescription: "Service Description",
        serviceAsJson: "serviceAsJson",
        referrer: "referrer",
        fullName: "fullName",
        hasSpecialNeeds: "Has Special Needs",
        email: "someone@email.com",
        phone: "01211112222",
        text: "01211112222",
        dateRecieved: DateTime.SpecifyKind(new DateTime(2023, 1, 2), DateTimeKind.Utc),
        requestNumber: 1L,
        reasonForSupport: "Reason For Support",
        reasonForRejection: "Reason for Rejection",
        new List<ReferralStatusDto>()
        {
            new ReferralStatusDto("9efb3595-557d-4905-a330-a9a21a8ec23b", "Accept Connection")
        }
        );

        ReferralDto referralDto2 = new ReferralDto(
        id: "9efb3595-557d-4905-a330-a9a21a8ec23b",
        organisationId: "442e1d35-5563-455a-bbd4-b3432be1508f",
        serviceId: "a8ea6d74-0619-4573-b0a6-d4cca995b331",
        serviceName: "Service Name 1",
        serviceDescription: "Service Description 1",
        serviceAsJson: "serviceAsJson 1",
        referrer: "referrer",
        fullName: "fullName 1",
        hasSpecialNeeds: "Has Special Needs 1",
        email: "someone1@email.com",
        phone: "01211112223",
        text: "01211112223",
        dateRecieved: DateTime.SpecifyKind(new DateTime(2023, 1, 1), DateTimeKind.Utc),
        requestNumber: 1L,
        reasonForSupport: "Reason For Support",
        reasonForRejection: "Reason for Rejection",
        new List<ReferralStatusDto>()
        {
            new ReferralStatusDto("9efb3595-557d-4905-a330-a9a21a8ec23b", "Connection Made")
        }
        );

        PaginatedList<ReferralDto> paginatedList = new PaginatedList<ReferralDto>(items: new List<ReferralDto> { referralDto, referralDto2 }, 1, 1, 2);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "Professional"),
            new Claim(ClaimTypes.Role, "Professional"),
            new Claim("OpenReferralOrganisationId", referralDto.OrganisationId),
        }, "mock"));
        referralDashboardModel.PageContext.HttpContext = new DefaultHttpContext() { User = user };
        mockReferralClientService.Setup(x => x.GetReferralsByReferrer(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<bool?>())).ReturnsAsync(paginatedList);

        //Act
        await referralDashboardModel.OnPostAsync(false, default!, true, statusDirection);

        //Assert
        if (statusDirection == "ascending")
            referralDashboardModel.ReferralList.Items[0].Should().BeEquivalentTo(referralDto2);
        else
            referralDashboardModel.ReferralList.Items[0].Should().BeEquivalentTo(referralDto);
    }
}
