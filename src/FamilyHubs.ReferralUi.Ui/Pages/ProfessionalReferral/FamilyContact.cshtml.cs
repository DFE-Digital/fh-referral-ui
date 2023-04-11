using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class FamilyContactModel : PageModel
{
    private readonly IRedisCacheService _cacheService;

    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;


    [BindProperty]
    public string FullName { get; set; } = string.Empty;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public FamilyContactModel(IRedisCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public void OnGet()
    {
        string userKey = _cacheService.GetUserKey();
        ConnectWizzardViewModel model = _cacheService.RetrieveConnectWizzardViewModel(userKey);

        Id = model.ServiceId;
        Name = model.ServiceName;
       
        if (!string.IsNullOrEmpty(model.FullName))
            FullName = model.FullName;
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            if (FullName == null || FullName.Trim().Length == 0 || FullName.Length > 255)
                ValidationValid = false;

            return Page();
        }

        string userKey = _cacheService.GetUserKey();
        ConnectWizzardViewModel model = _cacheService.RetrieveConnectWizzardViewModel(userKey);
        model.FullName = FullName;
        _cacheService.StoreConnectWizzardViewModel(userKey, model);

        return RedirectToPage("/ProfessionalReferral/ContactDetails", new
        {
        });

    }
}
