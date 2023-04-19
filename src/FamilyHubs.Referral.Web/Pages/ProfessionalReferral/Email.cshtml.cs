using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyHubs.Referral.Core.Helper;
using System.ComponentModel.DataAnnotations;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class EmailModel : PageModel
{
    private readonly IDistributedCacheService _distributedCacheService;
    public PartialTextBoxViewModel PartialTextBoxViewModel { get; set; } = new PartialTextBoxViewModel()
    {
        HeadingText = string.Empty,
        ErrorId = "error-summary-title",
        HintText = string.Empty,
        TextBoxLabel = "Email address",
        MainErrorText = "Enter an email address in the correct format, like name@example.com",
        TextBoxErrorText = "Enter an email address in the correct format, like name@example.com",
    };

    [EmailAddress]
    [BindProperty]
    public string TextBoxValue { get; set; } = string.Empty;

    public EmailModel(IDistributedCacheService distributedCacheService)
    {
        _distributedCacheService = distributedCacheService;
    }

    public void OnGet()
    {
        ConnectWizzardViewModel model = _distributedCacheService.RetrieveConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel);
        PartialTextBoxViewModel.HeadingText = $"What is the email address for {model.FullName}?";

        if (!string.IsNullOrEmpty(model.EmailAddress))
        {
            PartialTextBoxViewModel.TextBoxValue = model.EmailAddress;
            TextBoxValue = model.EmailAddress;
        }
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            PartialTextBoxViewModel.TextBoxValue = TextBoxValue;
            if (string.IsNullOrWhiteSpace(TextBoxValue?.Trim()))
                PartialTextBoxViewModel.ValidationValid = false;

            return Page();
        }

        if (TextBoxValue.Length > 255)
        {
            TextBoxValue = TextBoxValue.Truncate(252) ?? string.Empty;
        }

        ConnectWizzardViewModel model = _distributedCacheService.RetrieveConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel);
        model.EmailAddress = TextBoxValue;
        _distributedCacheService.StoreConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel, model);

        string destination = string.Empty;
        if (model.TelephoneSelected)
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
        else
        {
            destination = "ContactMethod";
        }

        return RedirectToPage($"/ProfessionalReferral/{destination}", new
        {
        });
    }
}
