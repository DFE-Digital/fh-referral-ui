using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.SharedKernel;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ServiceDirectory.Ui.Pages.Vcs;

public class ShowReferralSummaryModel : PageModel
{
    private readonly IReferralClientService _referralClientService;
    private readonly ITokenService _tokenService;

    public PaginatedList<ReferralDto> ReferralList { get; set; } = default!;

    public ShowReferralSummaryModel(IReferralClientService referralClientService, ITokenService tokenService)
    {
        _referralClientService = referralClientService;
        _tokenService = tokenService;
    }
    public async Task OnGet()
    {
        var organisationId = _tokenService.GetUsersOrganisationId();
        ReferralList = await _referralClientService.GetReferralsByOrganisationId(organisationId, 1, 99);
    }
}
