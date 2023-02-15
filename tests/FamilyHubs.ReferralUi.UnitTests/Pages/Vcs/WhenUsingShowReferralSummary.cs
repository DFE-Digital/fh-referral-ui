using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Ui.Pages.Vcs;
using FamilyHubs.SharedKernel;
using FluentAssertions;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.Vcs;

public class WhenUsingShowReferralSummary
{
    [Fact]
    public async Task ThenOnGetShowReferralSummary()
    {
        //Arrange 
        Mock<IReferralClientService> mockReferralClientService = new Mock<IReferralClientService>();
        Mock<ITokenService> mockTokenService = new Mock<ITokenService>();
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
        reasonForSupport: "Reason For Support",
        reasonForRejection: "Reason for Rejection",
        new List<ReferralStatusDto>()
        );

        PaginatedList<ReferralDto> paginatedList = new PaginatedList<ReferralDto>(items: new List<ReferralDto> { referralDto }, 1, 1, 2);

        mockReferralClientService.Setup(x => x.GetReferralsByOrganisationId(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(paginatedList);
        mockTokenService.Setup(x => x.GetUsersOrganisationId()).Returns(referralDto.OrganisationId);
        ShowReferralSummaryModel showReferralSummaryModel = new ShowReferralSummaryModel(mockReferralClientService.Object, mockTokenService.Object);

        //Act
        await showReferralSummaryModel.OnGet();

        //Assert
        showReferralSummaryModel.ReferralList.Should().BeEquivalentTo(paginatedList);
    }
}
