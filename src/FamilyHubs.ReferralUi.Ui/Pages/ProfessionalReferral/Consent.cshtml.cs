using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class ConsentModel : PageModel
{
    private readonly ICacheService _cacheService;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    [BindProperty]
    public string IsConsentGiven { get; set; } = default!;


    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public ConsentModel(ICacheService cacheService)
    {
        _cacheService = cacheService;
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
            if (User!= null && User.Identity != null && !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/ProfessionalReferral/SignIn", new
                {
                    id,
                    name
                });

            }

            string userKey = _cacheService.GetUserKey();
            ConnectWizzardViewModel model = _cacheService.RetrieveConnectWizzardViewModel(userKey);
            model.ServiceId = Id;
            model.ServiceName = name;
            model.HaveConcent = false;
            _cacheService.StoreConnectWizzardViewModel(userKey, model);

            return RedirectToPage("/ProfessionalReferral/FamilyContact", new
            {
            });

        }

        return RedirectToPage("/ProfessionalReferral/ConsentShutter", new
        {
        });

    }
    
}
