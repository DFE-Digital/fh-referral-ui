using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Helper;
using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public interface ISingleTextboxPageModel
{
    //todo: get rid of text and label
    public string HeadingText { get; set; }
    //todo: no need for this?
    public string ErrorId { get; set; }
    public string HintText { get; set; }
    public string TextBoxLabel { get; set; }
    public string MainErrorText { get; set; }
    //todo: default to mainerror
    public string? TextBoxErrorText { get; set; }
    public string? TextBoxValue { get; set; }
    public bool ValidationValid { get; set; }
}

public class SupportDetailsModel : PageModel, ISingleTextboxPageModel
{
    private readonly IConnectionRequestDistributedCache _connectionRequestDistributedCache;
    public string? ServiceId { get; private set; }
    public string? ServiceName { get; private set; }

    //todo: separate static with changing
    //public PartialTextBoxViewModel PartialTextBoxViewModel { get; } = new()
    //{
    public string ErrorId { get; set; } = "error-summary-title";
    public string HeadingText { get; set; } = "Who should the service contact in the family?";
    public string HintText { get; set; } = "This must be a person aged 16 or over.";
    public string TextBoxLabel { get; set; } = "Full name";
    public string MainErrorText { get; set; } = "Enter a full name";
    public string? TextBoxErrorText { get; set; } = "Enter a full name";
    public bool ValidationValid { get; set; } = true;

    [BindProperty]
    public string? TextBoxValue { get; set; } = string.Empty;

    public SupportDetailsModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
    {
        _connectionRequestDistributedCache = connectionRequestDistributedCache;
    }

    public async Task OnGetAsync(string serviceId, string serviceName)
    {
        //todo:
        //Fixes Session Changing between requests 
        HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });

        ServiceId = serviceId;
        ServiceName = serviceName;

        var model = await _connectionRequestDistributedCache.GetAsync();

        if (!string.IsNullOrEmpty(model?.FamilyContactFullName))
        {
            //todo: two?
            //PartialTextBoxViewModel.TextBoxValue = model.FamilyContactFullName;
            TextBoxValue = model.FamilyContactFullName;
        }
    }

    public async Task<IActionResult> OnPostAsync(string serviceId, string serviceName)
    {
        if (!ModelState.IsValid)
        {
            //PartialTextBoxViewModel.TextBoxValue = TextBoxValue;
            if (string.IsNullOrWhiteSpace(TextBoxValue))
                ValidationValid = false;

            return Page();
        }

        //todo: could it be null?
        if (TextBoxValue!.Length > 255)
        {
            TextBoxValue = TextBoxValue.Truncate(252);
        }

        var model = await _connectionRequestDistributedCache.GetAsync()
                    ?? new ConnectionRequestModel
                    {
                        ServiceId = serviceId,
                        ServiceName = serviceName
                    };

        model.FamilyContactFullName = TextBoxValue;
        await _connectionRequestDistributedCache.SetAsync(model);

        return RedirectToPage("/ProfessionalReferral/WhySupport",new
        {
            serviceId,
            serviceName
        });
    }
}
