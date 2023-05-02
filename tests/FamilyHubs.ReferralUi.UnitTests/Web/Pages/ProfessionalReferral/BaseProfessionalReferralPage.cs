using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class BaseProfessionalReferralPage
{
    public Mock<IConnectionRequestDistributedCache> ReferralDistributedCache;
    public readonly ConnectionRequestModel ConnectionRequestModel;

    public const string EmailAddress = "example.com";

    public BaseProfessionalReferralPage()
    {
        ConnectionRequestModel = new ConnectionRequestModel
        {
            ServiceId = "ServiceId",
            ServiceName = "ServiceName",
            FamilyContactFullName = "FamilyContactFullName",
            Reason = "Reason For Support",
            EmailSelected = true,
            TelephoneSelected = true,
            TextphoneSelected = true,
            LetterSelected = true,
            EmailAddress = EmailAddress
        };

        ReferralDistributedCache = new Mock<IConnectionRequestDistributedCache>(MockBehavior.Strict);
        ReferralDistributedCache.Setup(x => x.SetAsync(It.IsAny<ConnectionRequestModel>())).Returns(Task.CompletedTask);
        ReferralDistributedCache.Setup(x => x.GetAsync()).ReturnsAsync(ConnectionRequestModel);
    }
}