using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.Referrals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class ReferralDashboardDetailsModel : PageModel
{
    private readonly IReferralClientService _referralClientService;

    public ReferralDto Referral { get; set; } = default!;

    [BindProperty]
    public string ReferralId { get; set; } = default!;

    [BindProperty]
    public string? SelectedStatus { get; set; } = default!;

    public List<SelectListItem> StatusOptions { get; set; } = new()
    {
        new SelectListItem("Initial Connection", "Initial Connection"),
        new SelectListItem("Request More Information", "Request More Information"),
        new SelectListItem("Reject Connection", "Reject Connection"),
        new SelectListItem("Accept Connection", "Accept Connection"),
        new SelectListItem("Connection Made", "Connection Made")
    };

    public ReferralDashboardDetailsModel(IReferralClientService referralClientService)
    {
        _referralClientService = referralClientService;
    }
    public async Task OnGet(string id)
    {
        await Init(id);  
    }

    public async Task OnPost()
    {
        if (!string.IsNullOrEmpty(SelectedStatus))
        {
            var status = await _referralClientService.SetReferralStatusReferral(ReferralId, SelectedStatus);
        }

        await Init(ReferralId);
    }

    public async Task<IActionResult> OnPostEditDetails()
    {
        ReferralDto? dto = await _referralClientService.GetReferralById(ReferralId);
        if (dto != null) 
        {
            return RedirectToPage("/ProfessionalReferral/CheckReferralDetails", new
            {
                id = dto.ServiceId,
                name = dto.ServiceName,
                fullName = dto.FullName,
                email = dto.Email,
                telephone = dto.Phone,
                textphone = dto.Text,
                reasonForSupport = dto.ReasonForSupport,
                referralId = dto.Id
            });
        }

        return Page();
    }
    

    private async Task Init(string id)
    {
        var referral = await _referralClientService.GetReferralById(id);
        if (referral != null)
        {
            ReferralId = referral.Id;
            Referral = referral;
            var currentStatus = Referral.Status.LastOrDefault();
            if (currentStatus != null)
            {
                if (currentStatus.Status == "Inital-Referral")
                {
                    SelectedStatus = "Initial Connection";
                }
                else
                {
                    SelectedStatus = currentStatus.Status;
                }
            }
            else
            {
                SelectedStatus = "Initial Connection";
            }

        }
    }
}
