using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class TelephoneModel : PageModel
{
    private readonly IDistributedCacheService _distributedCacheService;
    public PartialTextBoxViewModel PartialTextBoxViewModel { get; set; } = new PartialTextBoxViewModel()
    {
        HeadingText = string.Empty,
        ErrorId = "error-summary-title",
        HintText = string.Empty,
        TextBoxLabel = "UK telephone number",
        MainErrorText = "Enter a UK telephone number",
        TextBoxErrorText = "Enter a UK telephone number",
    };

    [BindProperty]
    public string TextBoxValue { get; set; } = string.Empty;

    public TelephoneModel(IDistributedCacheService distributedCacheService)
    {
        _distributedCacheService = distributedCacheService;
    }

    public void OnGet()
    {
        ConnectWizzardViewModel model = _distributedCacheService.RetrieveConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel);
        PartialTextBoxViewModel.HeadingText = $"What telephone number should the service use to call {model.FullName}?";

        if (!string.IsNullOrEmpty(model.TelephoneNumber))
        {
            PartialTextBoxViewModel.TextBoxValue = model.TelephoneNumber;
            TextBoxValue = model.TelephoneNumber;
        }
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid || !ValidatatePhonenumber(TextBoxValue))
        {
            PartialTextBoxViewModel.TextBoxValue = TextBoxValue;
            PartialTextBoxViewModel.ValidationValid = false;

            return Page();
        }

        ConnectWizzardViewModel model = _distributedCacheService.RetrieveConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel);
        model.EmailAddress = TextBoxValue;
        _distributedCacheService.StoreConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel, model);

        string destination = string.Empty;
        if (model.TextPhoneSelected)
        {
            destination = "Textphone";
        }
        else if (model.LetterSelected)
        {
            destination = "Letter";
        }
        else
        {
            destination = "ContactMethod";
        }

        return RedirectToPage($"/ProfessionalReferral/{destination}", new
        {
        });
    }

    private bool ValidatatePhonenumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            return false;
        }
        for (int i = 0; i < phoneNumber.Length; i++)
        {
            if (Char.IsDigit(phoneNumber[i]) || phoneNumber[i] == ' ' || phoneNumber[i] == '+')
            {
                continue;
            }

            return false;
        }

        return true;
    }
}
