using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SafeguardingModel : ProfessionalReferralModel
{
    public SafeguardingModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
        : base(connectionRequestDistributedCache, ConnectJourneyPage.Safeguarding)
    {
    }

    protected override async Task<IActionResult> OnSafeGetAsync()
    {
        await ConnectionRequestCache.RemoveAsync(ProfessionalUser.Email);

        return Page();
    }
}