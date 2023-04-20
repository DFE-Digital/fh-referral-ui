using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ContactDetailsModel : PageModel
{
    private readonly IDistributedCacheService _distributedCacheService;
    public bool ValidationValid { get; private set; } = true;
    public string? FullName { get; set; }

    [BindProperty]
    public string? Email { get; set; }

    [BindProperty]
    public string? Telephone { get; set; }

    [BindProperty]
    public string? Textphone { get; set; }

    [BindProperty]
    public string? Letter { get; set; }

    public ContactDetailsModel(IDistributedCacheService distributedCacheService)
    {
        _distributedCacheService = distributedCacheService;
    }
    public void OnGet()
    {
        ConnectWizzardViewModel model = _distributedCacheService.RetrieveConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel);
        FullName = !string.IsNullOrEmpty(model.FullName) ? model.FullName : "Family";
        Email = (model.EmailSelected) ? "Email" : null;
        Telephone = (model.TelephoneSelected) ? "Telephone" : null;
        Textphone = (model.TextPhoneSelected) ? "Textphone" : null;
        Letter = (model.LetterSelected) ? "Letter" : null;
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid || (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Telephone) && string.IsNullOrEmpty(Textphone) && string.IsNullOrEmpty(Letter)))
        {
            ValidationValid = false;
            
            return Page();
        }
        
        ConnectWizzardViewModel model = _distributedCacheService.RetrieveConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel);
        model.EmailSelected = !string.IsNullOrEmpty(Email);
        model.TelephoneSelected = !string.IsNullOrEmpty(Telephone);
        model.TextPhoneSelected = !string.IsNullOrEmpty(Textphone);
        model.LetterSelected = !string.IsNullOrEmpty(Letter);
        _distributedCacheService.StoreConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel,model);

        string destination = string.Empty;
        if (model.EmailSelected)
        {
            destination = "Email";
        }
        else if (model.TelephoneSelected) 
        {
            destination = "Telephone";
        }
        else if (model.TextPhoneSelected)
        {
            destination = "Textphone";
        }
        else if (model.LetterSelected)
        {
            destination = "Letter";
        }

        return RedirectToPage($"/ProfessionalReferral/{destination}", new
        {
        });
    }
}
