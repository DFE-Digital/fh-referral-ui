using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class ConsentModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;
    public string ReferralId { get; set; } = default!;

    [BindProperty]
    public string IsConsentGiven { get; set; } = default!;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public ConsentModel(IRedisCacheService redisCacheService)
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
        Id = model.ServiceId;
        Name = model.ServiceName;
        ReferralId = model.ReferralId;
    }

    public IActionResult OnPost(string id, string name, string referralId)
    {
        ModelState.Remove("ReferralId");

        if (!ModelState.IsValid || IsConsentGiven == null)
        {
            Id = id;
            Name = name;
            ReferralId = referralId;
            ValidationValid = false;
            return Page();
        }

        string userKey = _redisCacheService.GetUserKey();
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);

        if (string.Compare(IsConsentGiven, "yes", StringComparison.OrdinalIgnoreCase) == 0)
        {
            model.HaveConcent = true;
            _redisCacheService.StoreConnectWizzardViewModel(userKey, model);
            return RedirectToPage("/ProfessionalReferral/FamilyContact", new
            {
                id = id,
                name = name,
                referralId = referralId
            });
        }

        model.HaveConcent = false;
        _redisCacheService.StoreConnectWizzardViewModel(userKey, model);

        return RedirectToPage("/ProfessionalReferral/ConsentShutter", new
        {
        });

    }
    
}
