﻿using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class BaseProfessionalReferralPage
{
    public Mock<IConnectionRequestDistributedCache> ReferralDistributedCache;
    public readonly ConnectionRequestModel ConnectionRequestModel;

    public const string EmailAddress = "example.com";
    public const string Telephone = "07700 900000";
    public const string Text = "07700 900001";

    public BaseProfessionalReferralPage()
    {
        ConnectionRequestModel = new ConnectionRequestModel
        {
            ServiceId = "ServiceId",
            ServiceName = "ServiceName",
            FamilyContactFullName = "FamilyContactFullName",
            Reason = "Reason For Support",
            ContactMethodsSelected = new[] { true, true, true, true },
            EmailAddress = EmailAddress,
            TelephoneNumber = Telephone,
            TextphoneNumber = Text
        };

        ReferralDistributedCache = new Mock<IConnectionRequestDistributedCache>(MockBehavior.Strict);
        ReferralDistributedCache.Setup(x => x.SetAsync(It.IsAny<ConnectionRequestModel>())).Returns(Task.CompletedTask);
        ReferralDistributedCache.Setup(x => x.GetAsync()).ReturnsAsync(ConnectionRequestModel);
    }
}