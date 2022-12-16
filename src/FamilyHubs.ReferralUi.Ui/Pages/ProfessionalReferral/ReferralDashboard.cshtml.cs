using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.Referrals;
using FamilyHubs.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class ReferralDashboardModel : PageModel
{
    private readonly IReferralClientService _referralClientService;

    public PaginatedList<ReferralDto> ReferralList { get; set; } = default!;

    public ReferralDashboardModel(IReferralClientService referralClientService)
    {
        _referralClientService = referralClientService;
    }

    public async Task OnGet()
    {
        if (User != null && User.Identity != null)
        {
            ReferralList = await _referralClientService.GetReferralsByReferrer(User?.Identity?.Name ?? "BtlPro@email.com", 1, 999999);
        }
        else
        {
            ReferralList = await _referralClientService.GetReferralsByReferrer("BtlPro@email.com", 1, 999999);
        }
        
    }
}
