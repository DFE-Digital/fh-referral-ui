using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingSafeguardingModel : BaseProfessionalReferralPage
{
    [Fact]
    public async Task ThenOnGetSetsIdAndName()
    {
        var safeguardingModel = new SafeguardingModel(ReferralDistributedCache.Object);

        //Act
        await safeguardingModel.OnGetAsync("Id");

        safeguardingModel.ServiceId.Should().Be("Id");
    }
}
