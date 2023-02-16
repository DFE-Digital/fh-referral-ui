using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class ReferralDashboardDetailsModel : PageModel
{
    private readonly IReferralClientService _referralClientService;
    private readonly IRedisCacheService _redisCacheService;

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

    public ReferralDashboardDetailsModel(IReferralClientService referralClientService, IRedisCacheService redisCacheService)
    {
        _referralClientService = referralClientService;
        _redisCacheService = redisCacheService;
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

        ReferralDto? dto = await _referralClientService.GetReferralById(ReferralId);
        if (dto != null) 
        {
            dto.ReasonForRejection = ReasonForRejection;
            await _referralClientService.UpdateReferral(dto);
        }

        if (!string.IsNullOrEmpty(SelectedStatus))
        {
            await _referralClientService.SetReferralStatusReferral(ReferralId, SelectedStatus);
        }

        await Init(ReferralId);
    }

    public async Task<IActionResult> OnPostEditDetails()
    {
        ReferralDto? dto = await _referralClientService.GetReferralById(ReferralId);
        if (dto != null) 
        {
            string userKey = _redisCacheService.GetUserKey();
            ConnectWizzardViewModel model = new ConnectWizzardViewModel
            {
                ServiceId = dto.ServiceId,
                ServiceName = dto.ServiceName,
                ReferralId = dto.Id,
                AnyoneInFamilyBeingHarmed = false,
                HaveConcent = true,
                FullName = dto.FullName,
                EmailAddress = dto.Email,
                Telephone = dto.Phone,
                Textphone = dto.Text,
                ReasonForSupport = dto.ReasonForSupport
            };
            
            _redisCacheService.StoreConnectWizzardViewModel(userKey, model);

            return RedirectToPage("/ProfessionalReferral/CheckReferralDetails", new
            {
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
            ReasonForRejection = referral.ReasonForRejection ?? string.Empty;
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
