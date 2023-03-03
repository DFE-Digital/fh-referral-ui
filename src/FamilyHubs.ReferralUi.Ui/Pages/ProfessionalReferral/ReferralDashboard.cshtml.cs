using FamilyHubs.ReferralUi.Ui.Models;
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

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 5;

    [BindProperty]
    public string? SearchText { get; set; }

    [BindProperty]
    public string OrganisationId { get; set; } = string.Empty;

    [BindProperty]
    public string DateRecievedStringSortDirection { get; set; } = "ascending";

    [BindProperty]
    public string StatusStringSortDirection { get; set; } = "ascending";



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
            
            ReferralList = await _referralClientService.GetReferralsByOrganisationId(organisationId, CurrentPage, PageSize, SearchText, true);
            return;
        }

        ReferralList = await _referralClientService.GetReferralsByReferrer(User?.Identity?.Name ?? string.Empty, CurrentPage, PageSize, SearchText, false);        
    }

    public async Task OnPostAsync(bool sortDateRecieved, string dateRecievedDirection, bool sortStatus , string statusDirection)
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

            await GetReferralsByOrganisationId(sortDateRecieved, dateRecievedDirection, sortStatus, statusDirection);

            return;
        }

        await GetReferralsByReferrer(sortDateRecieved, dateRecievedDirection, sortStatus, statusDirection);
    }

    private async Task GetReferralsByOrganisationId(bool sortDateRecieved, string dateRecievedDirection, bool sortStatus, string statusDirection)
    {
        ReferralList = await _referralClientService.GetReferralsByOrganisationId(OrganisationId, CurrentPage, PageSize, SearchText, true);

        if (sortDateRecieved)
        {
            if (dateRecievedDirection == "ascending")
            {
                ReferralList.Items = ReferralList.Items.OrderByDescending(x => x.GetDateRecieved()).ToList();
                DateRecievedStringSortDirection = "descending";
            }
            else
            {
                ReferralList.Items = ReferralList.Items.OrderBy(x => x.GetDateRecieved()).ToList();
                DateRecievedStringSortDirection = "ascending";
            }

        }

        if (sortStatus)
        {
            if (statusDirection == "ascending")
            {
                ReferralList.Items = ReferralList.Items.OrderByDescending(x => x.GetStatus()).ToList();
                DateRecievedStringSortDirection = "descending";
            }
            else
            {
                ReferralList.Items = ReferralList.Items.OrderBy(x => x.GetStatus()).ToList();
                DateRecievedStringSortDirection = "ascending";
            }
        }
    }

    private async Task GetReferralsByReferrer(bool sortDateRecieved, string dateRecievedDirection, bool sortStatus, string statusDirection)
    {
        ReferralList = await _referralClientService.GetReferralsByReferrer(User?.Identity?.Name ?? string.Empty, CurrentPage, PageSize, SearchText, false);
        
        if (sortDateRecieved)
        {
            if (dateRecievedDirection == "ascending")
            {
                ReferralList.Items = ReferralList.Items.OrderByDescending(x => x.GetDateRecieved()).ToList();
                DateRecievedStringSortDirection = "descending";
            }
            else
            {
                ReferralList.Items = ReferralList.Items.OrderBy(x => x.GetDateRecieved()).ToList();
                DateRecievedStringSortDirection = "ascending";
            }

        }

        if (sortStatus)
        {
            if (statusDirection == "ascending")
            {
                ReferralList.Items = ReferralList.Items.OrderByDescending(x => x.GetStatus()).ToList();
                DateRecievedStringSortDirection = "descending";
            }
            else
            {
                ReferralList.Items = ReferralList.Items.OrderBy(x => x.GetStatus()).ToList();
                DateRecievedStringSortDirection = "ascending";
            }
        }
    }
}
