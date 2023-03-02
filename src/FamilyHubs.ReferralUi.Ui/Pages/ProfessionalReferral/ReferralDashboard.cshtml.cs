using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class ReferralDashboardModel : PageModel
{
    private readonly IReferralClientService _referralClientService;

    public PaginatedList<ReferralDto> ReferralList { get; set; } = default!;

    [BindProperty]
    public string? SearchText { get; set; }

    [BindProperty]
    public string OrganisationId { get; set; } = string.Empty;

    public ReferralDashboardModel(IReferralClientService referralClientService)
    {
        _referralClientService = referralClientService;
    }

    public async Task OnGetAsync(string organisationId)
    {
        OrganisationId = organisationId;
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
            
            ReferralList = await _referralClientService.GetReferralsByOrganisationId(organisationId, 1, 999999, default!, default!);
            return;
        }

        ReferralList = await _referralClientService.GetReferralsByReferrer(User?.Identity?.Name ?? string.Empty, 1, 999999);        
    }

    public async Task OnPostAsync()
    {
        if (User.IsInRole("VCSAdmin"))
        {
            if (string.IsNullOrEmpty(OrganisationId))
            {
                var claim = User.Claims.FirstOrDefault(x => x.Type == "OpenReferralOrganisationId");
                if (claim != null)
                {
                    OrganisationId = claim.Value;
                }
            }

            ReferralList = await _referralClientService.GetReferralsByOrganisationId(OrganisationId, 1, 999999, default!, default!);
            return;
        }

        ReferralList = await _referralClientService.GetReferralsByReferrer(User?.Identity?.Name ?? string.Empty, 1, 999999);
    }
}
