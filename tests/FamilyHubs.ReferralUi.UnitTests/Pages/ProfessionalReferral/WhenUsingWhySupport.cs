//using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
//using FluentAssertions;
//using Microsoft.AspNetCore.Mvc;
//using Moq;

//namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

//public class WhenUsingWhySupport
//{
//    private readonly WhySupportModel _whySupportModel;
//    private readonly Mock<IDistributedCacheService> _mockCacheService;
//    private readonly ConnectWizzardViewModel _connectWizzardViewModel;
//    public WhenUsingWhySupport()
//    {
//        _connectWizzardViewModel = new ConnectWizzardViewModel
//        {
//            ServiceId = "Service Id",
//            ServiceName = "Service Name",
//            FullName = "Full Name",
//            ReasonForSupport = "Reason for Support"
//        };
//        _mockCacheService = new Mock<IDistributedCacheService>();
//        _whySupportModel = new WhySupportModel(_mockCacheService.Object);
//    }

//    [Fact]
//    public void ThenOnGetWhySupport()
//    {
//        //Arrange
//        _mockCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);

//        //Act
//        _whySupportModel.OnGet();

//        //Assert
//        _whySupportModel.ServiceId.Should().Be(_connectWizzardViewModel.ServiceId);
//        _whySupportModel.ServiceName.Should().Be(_connectWizzardViewModel.ServiceName);
//        _whySupportModel.TextAreaValue.Should().Be(_connectWizzardViewModel.ReasonForSupport);

//    }

//    [Fact]
//    public void ThenOnPostWhySupport()
//    {
//        //Arrange
//        _mockCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
//        int callBack = 0;
//        _mockCacheService.Setup(x => x.StoreConnectWizzardViewModel(It.IsAny<string>(),It.IsAny<ConnectWizzardViewModel>())).Callback(() => callBack++);
//        _whySupportModel.TextAreaValue = "Reason For Support";

//        //Act
//        var result = _whySupportModel.OnPost() as RedirectToPageResult;


//        //Assert
//        callBack.Should().Be(1);
//        ArgumentNullException.ThrowIfNull(result);
//        result.PageName.Should().Be("/ProfessionalReferral/ContactDetails");
//    }

//    [Theory]
//    [InlineData(default!)]
//    [InlineData(" ")]
//    public void ThenOnPostSupportDetailsWithEmptyFullName(string value)
//    {
//        //Arrange
//        _whySupportModel.TextAreaValue = value;
//        _whySupportModel.ModelState.AddModelError("Text Area", "Enter a Reason For Support");

//        //Act
//        _whySupportModel.OnPost();


//        //Assert
//        _whySupportModel.ValidationValid.Should().BeFalse();
//    }
//}
