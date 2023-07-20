using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingContactByPhone : BaseProfessionalReferralPage
{
    private readonly ContactByPhoneModel _contactByPhoneModel;

    public WhenUsingContactByPhone()
    {
        _contactByPhoneModel = new ContactByPhoneModel(ReferralDistributedCache.Object);
    }

    [Fact]
    public async Task GivenNoContactSelected_WhenOnPostWithModel_ThenRedirectToSelf()
    {
        // Arrange
        _contactByPhoneModel.Contact = null;

        // Act
        var result = await _contactByPhoneModel.OnPostAsync("1");

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        var redirectResult = (RedirectToPageResult)result;
        Assert.Equal("/ProfessionalReferral/ContactByPhone", redirectResult.PageName);
    }
}