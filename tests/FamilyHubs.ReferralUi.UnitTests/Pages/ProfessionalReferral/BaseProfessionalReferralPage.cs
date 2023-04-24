using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class BaseProfessionalReferralPage
{
    public Mock<IReferralDistributedCache> ReferralDistributedCache;
    public readonly ProfessionalReferralModel ProfessionalReferralModel;

    public BaseProfessionalReferralPage()
    {
        ProfessionalReferralModel = new ProfessionalReferralModel
        {
            ServiceId = "ServiceId",
            ServiceName = "ServiceName",
            FullName = "FullName",
        };

        ReferralDistributedCache = new Mock<IReferralDistributedCache>(MockBehavior.Strict);
        ReferralDistributedCache.Setup(x => x.SetProfessionalReferralAsync(It.IsAny<ProfessionalReferralModel>())).Returns(Task.CompletedTask);
        ReferralDistributedCache.Setup(x => x.GetProfessionalReferralAsync()).ReturnsAsync(ProfessionalReferralModel);
    }
}