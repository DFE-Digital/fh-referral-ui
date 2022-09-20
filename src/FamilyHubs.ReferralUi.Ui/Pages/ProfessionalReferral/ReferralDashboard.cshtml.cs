using FamilyHubs.ReferralApi.Common.Dto;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.SharedKernel;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

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
        ReferralList = await _referralClientService.GetReferralsByReferrer("CurrentUser", 1, 99);
    }
}
