using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Ui.Pages.Vcs;
using FluentAssertions;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.Vcs;

public class WhenUsingShowReferralDetails
{
    [Fact]
    public async Task ThenOnGetShowReferralDetails()
    {
        //Arrange 
        Mock<IReferralClientService> mockReferralClientService = new Mock<IReferralClientService>();
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
        dateRecieved: null,
         requestNumber: 0,
        reasonForSupport: "Reason For Support",
        reasonForRejection: "Reason for Rejection",
        new List<ReferralStatusDto>()
        );

        mockReferralClientService.Setup(x => x.GetReferralById(It.IsAny<string>())).ReturnsAsync( referralDto );
        ShowReferralDetailsModel showReferralDetailsModel = new ShowReferralDetailsModel(mockReferralClientService.Object);

        //Act
        await showReferralDetailsModel.OnGet(referralDto.Id);

        //Assert
        showReferralDetailsModel.Referral.Should().BeEquivalentTo( referralDto );
    }
}
