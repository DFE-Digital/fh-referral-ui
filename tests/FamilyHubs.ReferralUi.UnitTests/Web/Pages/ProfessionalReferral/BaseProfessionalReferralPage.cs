using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class BaseProfessionalReferralPage
{
    public Mock<IConnectionRequestDistributedCache> ReferralDistributedCache;
    public readonly ConnectionRequestModel ConnectionRequestModel;

    public const string Reason = "Reason";
    public const string EmailAddress = "example.com";
    public const string Telephone = "07700 900000";
    public const string Text = "07700 900001";
    public const string AddressLine1 = "AddressLine1";
    public const string AddressLine2 = "AddressLine2";
    public const string TownOrCity = "TownOrCity";
    public const string County = "County";
    public const string Postcode = "Postcode";
    public const string EngageReason = "EngageReason";

    public BaseProfessionalReferralPage()
    {
        ConnectionRequestModel = new ConnectionRequestModel
        {
            ServiceId = "ServiceId",
            FamilyContactFullName = "FamilyContactFullName",
            Reason = Reason,
            ContactMethodsSelected = new[] { true, true, true, true },
            EmailAddress = EmailAddress,
            TelephoneNumber = Telephone,
            TextphoneNumber = Text,
            AddressLine1 = AddressLine1,
            AddressLine2 = AddressLine2,
            TownOrCity = TownOrCity,
            County = County,
            Postcode = Postcode,
            EngageReason = EngageReason
        };

        ReferralDistributedCache = new Mock<IConnectionRequestDistributedCache>(MockBehavior.Strict);
        ReferralDistributedCache.Setup(x => x.SetAsync(It.IsAny<string>(),It.IsAny<ConnectionRequestModel>())).Returns(Task.CompletedTask);
        ReferralDistributedCache.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(ConnectionRequestModel);
    }
}