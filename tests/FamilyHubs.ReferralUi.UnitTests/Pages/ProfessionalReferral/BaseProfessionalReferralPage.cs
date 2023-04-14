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
            ReferralId = "ReferralId",
            AnyoneInFamilyBeingHarmed = false,
            HaveConcent = true,
            FullName = "FullName",
            EmailAddress = "someone@email.com",
            Telephone = "01212223333",
            Textphone = "0712345678",
            ReasonForSupport = "Reason For Support"
        };

        _mockICacheService = new Mock<IDistributedCacheService>(MockBehavior.Strict);
        _mockICacheService.Setup(x => x.StoreConnectWizzardViewModel(It.IsAny<string>(), It.IsAny<ConnectWizzardViewModel>()));
        _mockICacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
    }
}