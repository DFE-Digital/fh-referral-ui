using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
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

    public async Task OnGet(string organisationId)
    {
        var user = User;
        if (User.IsInRole("VCSAdmin")) 
        {
            if (string.IsNullOrEmpty(organisationId))
            {
                var claim = User.Claims.FirstOrDefault(x => x.Type == "OpenReferralOrganisationId");
                if (claim != null)
                {
                    organisationId = claim.Value;
                }
            }
            
            ReferralList = await _referralClientService.GetReferralsByOrganisationId(organisationId, 1, 999999);
            return;
        }

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
