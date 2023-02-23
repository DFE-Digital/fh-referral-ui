using FamilyHubs.ReferralUi.Ui.Services;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class BaseProfessionalReferralPage
{
    internal readonly Ui.Models.ConnectWizzardViewModel _connectWizzardViewModel;
    protected readonly Mock<IRedisCacheService> _mockIRedisCacheService;
    public BaseProfessionalReferralPage()
    {
        _connectWizzardViewModel = new Ui.Models.ConnectWizzardViewModel
        {
            ServiceId = "ServiceId",
            ServiceName = "ServiceName",
            ReferralId = "ReferralId",
            AnyoneInFamilyBeingHarmed = false,
            HaveConcent = true,
            FullName = "FullName",
            EmailAddress = "someone@email.com",
            Telephone = "01212223333",
            Textphone = "0712345678",
            ReasonForSupport = "Reason For Support"
        };

        _mockIRedisCacheService = new Mock<IRedisCacheService>(MockBehavior.Strict);
        _mockIRedisCacheService.Setup(x => x.GetUserKey()).Returns("UserKey");
        _mockIRedisCacheService.Setup(x => x.StoreConnectWizzardViewModel(It.IsAny<string>(), It.IsAny<Ui.Models.ConnectWizzardViewModel>()));
        _mockIRedisCacheService.Setup(x => x.ResetConnectWizzardViewModel(It.IsAny<string>()));
    }
}
