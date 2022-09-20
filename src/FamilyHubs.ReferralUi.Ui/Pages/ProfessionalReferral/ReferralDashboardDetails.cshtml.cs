using FamilyHubs.ReferralApi.Common.Dto;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class ReferralDashboardDetailsModel : PageModel
{
    private readonly IReferralClientService _referralClientService;

    public ReferralDto Referral { get; set; } = default!;

    public ReferralDashboardDetailsModel(IReferralClientService referralClientService)
    {
        _referralClientService = referralClientService;
    }
    public async Task OnGet(string id)
    {
        var referral = await _referralClientService.GetReferralById(id);
        if (referral != null)
            Referral = referral;
    }
}
