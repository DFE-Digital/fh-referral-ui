using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Helper;
using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SupportDetailsModel : PageModel
{
    private readonly IReferralDistributedCache _referralDistributedCache;
    public string ServiceId { get; private set; } = default!;
    public string ServiceName { get; private set; } = default!;

    //todo: separate static with changing
    public PartialTextBoxViewModel PartialTextBoxViewModel { get; } = new()
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

    public SupportDetailsModel(IReferralDistributedCache referralDistributedCache)
    {
        _referralDistributedCache = referralDistributedCache;
    }

    public async Task OnGetAsync(string serviceId, string serviceName)
    {
        //todo:
        //Fixes Session Changing between requests 
        HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });

        ServiceId = serviceId;
        ServiceName = serviceName;

        var model = await _referralDistributedCache.GetProfessionalReferralAsync();

        if (!string.IsNullOrEmpty(model?.FullName))
        {
            //todo: two?
            PartialTextBoxViewModel.TextBoxValue = model.FullName;
            TextBoxValue = model.FullName;
        }
    }

    public async Task<IActionResult> OnPostAsync(string serviceId, string serviceName)
    {
        if (!ModelState.IsValid)
        {
            PartialTextBoxViewModel.TextBoxValue = TextBoxValue;
            if (string.IsNullOrWhiteSpace(TextBoxValue))
                PartialTextBoxViewModel.ValidationValid = false;

            return Page();
        }

        if (TextBoxValue.Length > 255)
        {
            TextBoxValue = TextBoxValue.Truncate(255) ?? string.Empty;
        }

        var model = await _referralDistributedCache.GetProfessionalReferralAsync()
                    ?? new ProfessionalReferralModel
                    {
                        ServiceId = serviceId,
                        ServiceName = serviceName
                    };

        model.FullName = TextBoxValue;
        //todo: safe to use fire and forget?
        await _referralDistributedCache.SetProfessionalReferralAsync(model);

        return RedirectToPage("/ProfessionalReferral/WhySupport");
    }
}
