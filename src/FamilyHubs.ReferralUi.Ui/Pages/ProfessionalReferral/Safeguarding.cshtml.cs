using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class SafeguardingModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;

    [BindProperty]
    public string IsImmediateHarm { get; set; } = default!;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;


    public SafeguardingModel(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }
    public void OnGet()
    {
        string userKey = _redisCacheService.GetUserKey();
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);
        if (model.AnyoneInFamilyBeingHarmed != null)
        {
            IsImmediateHarm = model.AnyoneInFamilyBeingHarmed.Value ? "yes" : "no";
        }
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid || IsImmediateHarm == null)
        {
            ValidationValid = false;
            return Page();
        }

        string userKey = _redisCacheService.GetUserKey();
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);
        
        if (string.Compare(IsImmediateHarm, "no", StringComparison.OrdinalIgnoreCase) == 0)
        {
            model.AnyoneInFamilyBeingHarmed = false;
            _redisCacheService.StoreConnectWizzardViewModel(userKey, model);

            return RedirectToPage("/ProfessionalReferral/Consent", new
            {
            });
        }

        model.AnyoneInFamilyBeingHarmed = true;
        _redisCacheService.StoreConnectWizzardViewModel(userKey, model);

        return RedirectToPage("/ProfessionalReferral/SafeguardingShutter", new
        {
        });

    }
}
