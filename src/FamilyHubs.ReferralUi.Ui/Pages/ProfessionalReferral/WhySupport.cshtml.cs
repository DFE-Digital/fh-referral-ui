using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class WhySupportModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;

    [BindProperty]
    public string ReasonForSupport { get; set; } = default!;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public WhySupportModel(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }

    public void OnGet()
    {
        string userKey = _redisCacheService.GetUserKey();
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);
        if (!string.IsNullOrEmpty(model.ReasonForSupport))
            ReasonForSupport = model.ReasonForSupport;
    }

    public IActionResult OnPost()
    {
        if (ReasonForSupport == null || ReasonForSupport.Trim().Length == 0 || ReasonForSupport.Length > 500)
        {
            ValidationValid = false;
            return Page();
        }

        string userKey = _redisCacheService.GetUserKey();
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);
        model.ReasonForSupport = ReasonForSupport;
        _redisCacheService.StoreConnectWizzardViewModel(userKey, model);

        return RedirectToPage("/ProfessionalReferral/CheckReferralDetails", new
        {
        });

    }
}
