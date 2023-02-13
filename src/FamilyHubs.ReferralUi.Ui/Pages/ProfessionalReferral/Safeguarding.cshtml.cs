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
    public string ReferralId { get; set; } = default!;

    [BindProperty]
    public string IsImmediateHarm { get; set; } = default!;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    public SafeguardingModel(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }
    public void OnGet(string id, string name, string referralId)
    {
        Id = id;
        Name = name;
        ReferralId = referralId;

        string userKey = _redisCacheService.GetUserKey();
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);
        model.ServiceId = id;
        model.ServiceName= name;
        _redisCacheService.StoreConnectWizzardViewModel(userKey, model);
    }

    public IActionResult OnPost(string id, string name, string referralId)
    {
        ModelState.Remove("ReferralId");

        if (!ModelState.IsValid || IsImmediateHarm == null)
        {
            Id = id;
            Name = name;
            ReferralId = referralId;
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
                id = id,
                name = name,
                referralId = referralId
            });
        }

        model.AnyoneInFamilyBeingHarmed = true;
        _redisCacheService.StoreConnectWizzardViewModel(userKey, model);

        return RedirectToPage("/ProfessionalReferral/SafeguardingShutter", new
        {
        });

    }
}
