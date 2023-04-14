using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SupportDetailsModel : PageModel
{
    private readonly IDistributedCacheService _distributedCacheService;

    public string BackUrl { get; set; } = default!;

    [BindProperty]
    public string FullName { get; set; } = string.Empty;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public SupportDetailsModel(IDistributedCacheService distributedCacheService)
    {
        _distributedCacheService = distributedCacheService;
    }

    public void OnGet(string id, string name)
    {
        string encodeName = Uri.EscapeDataString(name);
;       BackUrl = $"/ProfessionalReferral/Consent?id={id}&name={encodeName}";

        ConnectWizzardViewModel model = _distributedCacheService.RetrieveConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel);

        model.ServiceId = id;
        model.ServiceName = name;

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

        ConnectWizzardViewModel model = _distributedCacheService.RetrieveConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel);
        model.FullName = FullName;
        _distributedCacheService.StoreConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel, model);

        return RedirectToPage("/ProfessionalReferral/ContactDetails", new
        {
        });

    }
}
