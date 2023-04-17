using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class WhySupportModel : PageModel
{
    private readonly IDistributedCacheService _distributedCacheService;

    [BindProperty]
    public string TextAreaValue { get; set; } = default!;

    public bool ValidationValid { get; set; } = true;

    public WhySupportModel(IDistributedCacheService distributedCacheService)
    {
        _distributedCacheService = distributedCacheService;
    }
    public void OnGet()
    {
        ConnectWizzardViewModel model = _distributedCacheService.RetrieveConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel);
        if (!string.IsNullOrEmpty(model.ReasonForSupport))
            TextAreaValue = model.ReasonForSupport;
    }

    public IActionResult OnPost()
    {
        if (TextAreaValue == null || TextAreaValue.Trim().Length == 0 || TextAreaValue.Length > 500)
        {
            ValidationValid = false;
            return Page();
        }

        ConnectWizzardViewModel model = _distributedCacheService.RetrieveConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel);
        model.ReasonForSupport = TextAreaValue;
        _distributedCacheService.StoreConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel, model);

        return RedirectToPage("/ProfessionalReferral/ContactDetails", new
        {
        });

    }
}
