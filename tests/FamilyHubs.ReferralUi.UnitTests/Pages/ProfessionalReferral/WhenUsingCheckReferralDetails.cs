using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Builders;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingCheckReferralDetails : BaseProfessionalReferralPage
{
    private readonly CheckReferralDetailsModel _checkReferralDetailsModel;

    
    private readonly Mock<ILocalOfferClientService> _localOfferClientService;
    private readonly Mock<IReferralClientService> _referralClientService;

    public WhenUsingCheckReferralDetails()
    {
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("UseRabbitMQ", "False")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        
        _localOfferClientService = new Mock<ILocalOfferClientService>();
        _referralClientService = new Mock<IReferralClientService>();
       // _localOfferClientService.Setup(x => x.GetLocalOfferById(It.IsAny<string>())).ReturnsAsync(GetTestCountyCouncilServicesDto("56e62852-1b0b-40e5-ac97-54a67ea957dc"));

        _referralClientService.Setup(x => x.CreateReferral(It.IsAny<ReferralDto>())).ReturnsAsync(_connectWizzardViewModel.ReferralId);
        _referralClientService.Setup(x => x.UpdateReferral(It.IsAny<ReferralDto>())).ReturnsAsync(_connectWizzardViewModel.ReferralId);

        _checkReferralDetailsModel = new CheckReferralDetailsModel(configuration, _localOfferClientService.Object, _referralClientService.Object,  _mockIRedisCacheService.Object);
    }

    [Fact]
    public void ThenOnGetCheckReferralDetails()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);

        //Act
        _checkReferralDetailsModel.OnGet();

        //Assert
        _checkReferralDetailsModel.Id.Should().Be(_connectWizzardViewModel.ServiceId);
        _checkReferralDetailsModel.Name.Should().Be(_connectWizzardViewModel.ServiceName);
        _checkReferralDetailsModel.ReferralId.Should().Be(_connectWizzardViewModel.ReferralId);
        _checkReferralDetailsModel.FullName.Should().Be(_connectWizzardViewModel.FullName);
        _checkReferralDetailsModel.Email.Should().Be(_connectWizzardViewModel.EmailAddress);
        _checkReferralDetailsModel.Telephone.Should().Be(_connectWizzardViewModel.Telephone);
        _checkReferralDetailsModel.Textphone.Should().Be(_connectWizzardViewModel.Textphone);
        _checkReferralDetailsModel.ReasonForSupport.Should().Be(_connectWizzardViewModel.ReasonForSupport);
        
    }

    [Fact]
    public async Task ThenOnPostCheckReferralDetails()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);


        //Act
        var result = await _checkReferralDetailsModel.OnPost() as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/ConfirmReferral");
    }

    [Fact]
    public async Task ThenOnPostCheckReferralDetails_Updates()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _referralClientService.Setup(x => x.GetReferralById(It.IsAny<string>())).ReturnsAsync(new ReferralDto(
            id: _connectWizzardViewModel.ReferralId,
            organisationId: "56e62852-1b0b-40e5-ac97-54a67ea957dc",
            serviceId: _connectWizzardViewModel.ServiceId,
            serviceName: _connectWizzardViewModel.ServiceName,
            serviceDescription: _connectWizzardViewModel.ServiceName,
            serviceAsJson: string.Empty, 
            referrer: string.Empty,
            fullName: string.Empty,
            hasSpecialNeeds: "no",
            email: default!,
            phone: default!,
            text: default!,
            dateRecieved: null,
            requestNumber: 0,
            reasonForSupport: string.Empty,
            reasonForRejection: string.Empty,
            new List<ReferralStatusDto>()));

        //Act
        var result = await _checkReferralDetailsModel.OnPost() as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/ConfirmReferral");
    }

    [Fact]
    public async Task ThenOnPostCheckReferralDetails_WithMissingReferralId()
    {
        //Arrange
        Ui.Models.ConnectWizzardViewModel model = new Ui.Models.ConnectWizzardViewModel
        {
            ServiceId = "ServiceId",
            ServiceName = "ServiceName",
            AnyoneInFamilyBeingHarmed = false,
            HaveConcent = true,
            FullName = "FullName",
            EmailAddress = "someone@email.com",
            Telephone = "01212223333",
            Textphone = "0712345678",
            ReasonForSupport = "Reason For Support"
        };
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(model);
        _referralClientService.Setup(x => x.GetReferralById(It.IsAny<string>())).ReturnsAsync(new ReferralDto(
            id: _connectWizzardViewModel.ReferralId,
            organisationId: "56e62852-1b0b-40e5-ac97-54a67ea957dc",
            serviceId: _connectWizzardViewModel.ServiceId,
            serviceName: _connectWizzardViewModel.ServiceName,
            serviceDescription: _connectWizzardViewModel.ServiceName,
            serviceAsJson: string.Empty,
            referrer: string.Empty,
            fullName: string.Empty,
            hasSpecialNeeds: "no",
            email: default!,
            phone: default!,
            text: default!,
            dateRecieved: null,
            requestNumber: 0,
            reasonForSupport: string.Empty,
            reasonForRejection: string.Empty,
            new List<ReferralStatusDto>()));

        //Act
        var result = await _checkReferralDetailsModel.OnPost() as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/ConfirmReferral");
    }
}
