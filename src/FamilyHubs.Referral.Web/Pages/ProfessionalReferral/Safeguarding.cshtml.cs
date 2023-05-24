using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SafeguardingModel : ProfessionalReferralModel
{
    private readonly IConnectionRequestDistributedCache _connectionRequestDistributedCache;

    //todo: store IConnectionRequestDistributedCache in base?
    public SafeguardingModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
        : base(ConnectJourneyPage.Safeguarding)
    {
        _connectionRequestDistributedCache = connectionRequestDistributedCache;
    }

    protected override async Task<IActionResult> OnSafeGetAsync()
    {
        await _connectionRequestDistributedCache.RemoveAsync(ProfessionalUser.Email);

        return Page();
    }
}