using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class WhySupportModel : PageModel
{
    private readonly ICacheService _cacheService;

    [BindProperty]
    public string ReasonForSupport { get; set; } = default!;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public WhySupportModel(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public void OnGet()
    {
        string userKey = _cacheService.GetUserKey();
        ConnectWizzardViewModel model = _cacheService.RetrieveConnectWizzardViewModel(userKey);
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

        string userKey = _cacheService.GetUserKey();
        ConnectWizzardViewModel model = _cacheService.RetrieveConnectWizzardViewModel(userKey);
        model.ReasonForSupport = ReasonForSupport;
        _cacheService.StoreConnectWizzardViewModel(userKey, model);

        return RedirectToPage("/ProfessionalReferral/CheckReferralDetails", new
        {
        });

    }
}
