using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class FamilyContactModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;

    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;


    [BindProperty]
    public string FullName { get; set; } = string.Empty;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public FamilyContactModel(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }

    public void OnGet()
    {
        string userKey = _redisCacheService.GetUserKey();
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);

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

        string userKey = _redisCacheService.GetUserKey();
        ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);
        model.FullName = FullName;
        _redisCacheService.StoreConnectWizzardViewModel(userKey, model);

        return RedirectToPage("/ProfessionalReferral/ContactDetails", new
        {
        });

    }
}
