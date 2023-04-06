using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class ConsentModel : PageModel
{
    private readonly IRedisCacheService _redisCacheService;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    [BindProperty]
    public string IsConsentGiven { get; set; } = default!;


    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public ConsentModel(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
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

            string userKey = _redisCacheService.GetUserKey();
            ConnectWizzardViewModel model = _redisCacheService.RetrieveConnectWizzardViewModel(userKey);
            model.ServiceId = Id;
            model.ServiceName = name;
            model.HaveConcent = false;
            _redisCacheService.StoreConnectWizzardViewModel(userKey, model);

            return RedirectToPage("/ProfessionalReferral/FamilyContact", new
            {
            });

        }

        return RedirectToPage("/ProfessionalReferral/ConsentShutter", new
        {
        });

    }
    
}
