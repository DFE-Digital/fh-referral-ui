using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SafeguardingModel : ProfessionalReferralModel
{
    public SafeguardingModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
        : base(ConnectJourneyPage.Safeguarding, connectionRequestDistributedCache)
    {
    }

    protected override async Task<IActionResult> OnSafeGetAsync()
    {
        await ConnectionRequestCache.RemoveAsync(ProfessionalUser.Email);

        return Page();
    }

    protected override async Task<IActionResult> OnSafePostAsync()
    {
        var model = new ConnectionRequestModel
        {
            ServiceId = ServiceId
        };

        await ConnectionRequestCache.SetAsync(ProfessionalUser.Email, model);

        return NextPage();
    }
}