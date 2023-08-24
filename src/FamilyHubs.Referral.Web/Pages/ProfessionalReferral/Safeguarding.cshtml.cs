﻿using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

[Authorize]
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