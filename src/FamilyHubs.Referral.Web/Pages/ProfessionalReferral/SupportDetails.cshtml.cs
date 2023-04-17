using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SupportDetailsModel : PageModel
{
    private readonly IDistributedCacheService _distributedCacheService;

    [BindProperty]
    public string BackUrl { get; set; } = default!;

    public PartialTextBoxViewModel PartialTextBoxViewModel { get; set; } = new PartialTextBoxViewModel()
    {
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
        string encodeName = Uri.EscapeDataString(serviceName);
        BackUrl = $"/ProfessionalReferral/Consent?serviceId={serviceId}&serviceName={encodeName}";

        ConnectWizzardViewModel model = _distributedCacheService.RetrieveConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel);

        model.ServiceId = serviceId;
        model.ServiceName = serviceName;

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
            if (TextBoxValue == null || TextBoxValue.Trim().Length == 0 || TextBoxValue.Length > 255)
                PartialTextBoxViewModel.ValidationValid = false;

            return Page();
        }

        ConnectWizzardViewModel model = _distributedCacheService.RetrieveConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel);
        model.FullName = TextBoxValue;
        _distributedCacheService.StoreConnectWizzardViewModel(TempStorageConfiguration.KeyConnectWizzardViewModel, model);

        return RedirectToPage("/ProfessionalReferral/ContactDetails", new
        {
        });

    }
}
