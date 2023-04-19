using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.Services;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class BaseProfessionalReferralPage
{
    internal readonly ConnectWizzardViewModel _connectWizzardViewModel;
    protected readonly Mock<IDistributedCacheService> _mockICacheService;
    public BaseProfessionalReferralPage()
    {
        _connectWizzardViewModel = new ConnectWizzardViewModel
        {
            ServiceId = "ServiceId",
            ServiceName = "ServiceName",
            FullName = "FullName",
            ReasonForSupport = "Reason For Support",
            EmailSelected = true,
            TelephoneSelected = true,
            TextPhoneSelected = true,
            LetterSelected = true,
        };

        _mockICacheService = new Mock<IDistributedCacheService>(MockBehavior.Strict);
        _mockICacheService.Setup(x => x.StoreConnectWizzardViewModel(It.IsAny<string>(), It.IsAny<ConnectWizzardViewModel>()));
        _mockICacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
    }
}
