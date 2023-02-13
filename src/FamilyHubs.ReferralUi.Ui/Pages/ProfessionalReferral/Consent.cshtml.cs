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

    [BindProperty]
    public string IsConsentGiven { get; set; } = default!;


    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public ConsentModel(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }

    public void OnGet()
    {
        string userKey = _redisCacheService.GetUserKey();
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);
        if (model.HaveConcent != null) 
        {
            IsConsentGiven = model.HaveConcent.Value ? "yes" : "no";
        }
        
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid || IsConsentGiven == null)
        {
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
            });
        }

        model.HaveConcent = false;
        _redisCacheService.StoreConnectWizzardViewModel(userKey, model);

        return RedirectToPage("/ProfessionalReferral/ConsentShutter", new
        {
        });

    }
    
}
