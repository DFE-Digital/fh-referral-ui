using FamilyHubs.Referral.Core.Helper;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SupportDetailsModel : PageModel
{
    private readonly IDistributedCacheService _distributedCacheService;

    public string ServiceId { get; private set; } = default!;
    public string ServiceName { get; private set; } = default!;

    public PartialTextBoxViewModel PartialTextBoxViewModel { get; set; } = new PartialTextBoxViewModel()
    {
        ErrorId = "error-summary-title",
        HeadingText = "Who should the service contact in the family?",
        HintText = "This must be a person aged 16 or over.",
        TextBoxLabel = "Full name",
        MainErrorText = "Enter a full name",
        TextBoxErrorText = "Enter a full name",
    };


    [BindProperty]
    public string TextBoxValue { get; set; } = string.Empty;

    public SupportDetailsModel(IDistributedCacheService distributedCacheService)
    {
        _distributedCacheService = distributedCacheService;
    }

    public void OnGet(string serviceId, string serviceName)
    {
        //Fixes Session Changing between requests 
        this.HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });

        ServiceId = serviceId;
        ServiceName = serviceName;

        ConnectWizzardViewModel model = _distributedCacheService.RetrieveConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel);
        model.ServiceId = serviceId;
        model.ServiceName = serviceName;
        _distributedCacheService.StoreConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel, model);

        if (!string.IsNullOrEmpty(model.FullName))
        {
            PartialTextBoxViewModel.TextBoxValue = model.FullName;
            TextBoxValue = model.FullName;
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
        model.FullName = TextBoxValue;
        _distributedCacheService.StoreConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel, model);

        return RedirectToPage("/ProfessionalReferral/WhySupport", new
        {
        });

    }
}
