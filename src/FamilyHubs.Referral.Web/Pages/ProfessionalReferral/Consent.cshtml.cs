using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ConsentModel : PageModel
{
    private readonly IDistributedCacheService _distributedCacheService;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    [BindProperty]
    public string IsConsentGiven { get; set; } = default!;


    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public ConsentModel(IDistributedCacheService distributedCacheService) 
    { 
        _distributedCacheService = distributedCacheService;
    }

    public void OnGet(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public IActionResult OnPost(string id, string name)
    {
        if (!ModelState.IsValid || IsConsentGiven == null)
        {
            ValidationValid = false;
            return Page();
        }


        if (string.Compare(IsConsentGiven, "yes", StringComparison.OrdinalIgnoreCase) == 0)
        {
            SetConsentState(true);

            return RedirectToPage("/ProfessionalReferral/FamilyContact", new
            {
                id,
                name
            });

        }

        SetConsentState(false);

        return RedirectToPage("/ProfessionalReferral/ConsentShutter", new
        {
        });

    }

    private void SetConsentState(bool isConsented = false) 
    {
        ConnectWizzardViewModel model = _distributedCacheService.RetrieveConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel);
        model.ServiceId = Id;
        model.ServiceName = Name;
        model.HaveConcent = isConsented;
        _distributedCacheService.StoreConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel, model);
    }
}
