using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FluentAssertions;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingReferralDashboardDetails : BaseProfessionalReferralPage
{
    private readonly ReferralDashboardDetailsModel _referralDashboardDetailsModel;
    private readonly Mock<IReferralClientService> _mockReferralClientService;


    public WhenUsingReferralDashboardDetails()
    {
        _mockReferralClientService = new Mock<IReferralClientService>();
        
        _referralDashboardDetailsModel = new ReferralDashboardDetailsModel(_mockReferralClientService.Object, _mockIRedisCacheService.Object);
    }

    [Theory]
    [InlineData(default)]
    [InlineData("")]
    [InlineData("Initial Connection")]
    [InlineData("Accept Connection")]
    [InlineData("Initial-Referral")]
    public async Task ThenOnGetReferralDashboard(string currentStatus)
    {
        //Arrange
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

        if (!string.IsNullOrEmpty(currentStatus))
        {
            referralDto.Status = new List<ReferralStatusDto>()
            {
                new ReferralStatusDto("f44e6827-502f-41df-9087-70f9a6cc8d81", currentStatus)
            };
        }

        _mockReferralClientService.Setup(x => x.GetReferralById(It.IsAny<string>())).ReturnsAsync(referralDto);
        

        //Act
        await _referralDashboardDetailsModel.OnGet(referralDto.Id);

        //Assert
        _referralDashboardDetailsModel.ReferralId.Should().Be(referralDto.Id);
        _referralDashboardDetailsModel.Referral.Should().BeEquivalentTo(referralDto);
        _referralDashboardDetailsModel.ReasonForRejection.Should().Be(referralDto.ReasonForRejection);
        
    }

    [Fact]
    public async Task ThenOnPostReferralDashboard()
    {
        //Arrange
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
        _referralDashboardDetailsModel.SelectedStatus = "Another Reason";
        _referralDashboardDetailsModel.ReasonForRejection = "My Reason";

        _mockReferralClientService.Setup(x => x.GetReferralById(It.IsAny<string>())).ReturnsAsync(referralDto);
        int updateCallback = 0;
        _mockReferralClientService.Setup(x => x.UpdateReferral(It.IsAny<ReferralDto>())).Callback(() => updateCallback++);
        int setStatusCallback = 0;
        _mockReferralClientService.Setup(x => x.SetReferralStatusReferral(It.IsAny<string>(), It.IsAny<string>())).Callback(() => setStatusCallback++);


        //Act
        await _referralDashboardDetailsModel.OnPost();

        //Assert
        updateCallback.Should().Be(1);
        setStatusCallback.Should().Be(1);
        _referralDashboardDetailsModel.ReasonForRejectionIsMissing.Should().BeFalse();

    }

    [Fact]
    public async Task ThenOnPostReferralDashboardWithConnectionRejected()
    {
        //Arrange
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
        _referralDashboardDetailsModel.SelectedStatus = "Reject Connection";
        _referralDashboardDetailsModel.ReasonForRejection = default!;

        _mockReferralClientService.Setup(x => x.GetReferralById(It.IsAny<string>())).ReturnsAsync(referralDto);
        int updateCallback = 0;
        _mockReferralClientService.Setup(x => x.UpdateReferral(It.IsAny<ReferralDto>())).Callback(() => updateCallback++);
        int setStatusCallback = 0;
        _mockReferralClientService.Setup(x => x.SetReferralStatusReferral(It.IsAny<string>(), It.IsAny<string>())).Callback(() => setStatusCallback++);


        //Act
        await _referralDashboardDetailsModel.OnPost();

        //Assert
        updateCallback.Should().Be(0);
        setStatusCallback.Should().Be(0);
        _referralDashboardDetailsModel.ReasonForRejectionIsMissing.Should().BeTrue();

    }
}
