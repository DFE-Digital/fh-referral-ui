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
    public string ReasonForRejection { get; set; } = default!;

    public bool ReasonForRejectionIsMissing { get; set; } = false;

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
        if (SelectedStatus == "Reject Connection" && string.IsNullOrEmpty(ReasonForRejection))
        {
            ReasonForRejectionIsMissing = true;
            await Init(ReferralId);
            return;
        }

        if (SelectedStatus == "Reject Connection")
        {
            ReferralDto? dto = await _referralClientService.GetReferralById(ReferralId);
            if (dto != null) 
            {
                dto.ReasonForRejection = ReasonForRejection;
                await _referralClientService.UpdateReferral(dto);
            }
        }

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
                if (currentStatus.Status == "Initial-Referral")
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
