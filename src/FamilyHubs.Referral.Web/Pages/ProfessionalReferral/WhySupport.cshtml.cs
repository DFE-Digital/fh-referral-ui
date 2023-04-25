using FamilyHubs.Referral.Core.DistributedCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class WhySupportModel : PageModel
{
    private readonly IReferralDistributedCache _referralDistributedCache;
    public string ServiceId { get; private set; } = default!;
    public string ServiceName { get; private set; } = default!;

    [BindProperty]
    public string TextAreaValue { get; set; } = default!;

    public bool ValidationValid { get; set; } = true;

    public WhySupportModel(IReferralDistributedCache referralDistributedCache)
    {
        _referralDistributedCache = referralDistributedCache;
    }

    public async Task<IActionResult> OnGetAsync(string serviceId)
    {
        var model = await _referralDistributedCache.GetProfessionalReferralAsync();
        if (model == null)
        {
            // session has expired and we don't have a model to work with
            // likely the user has come back to this page after a long time
            // send them back to the start of the journey
            return RedirectToPage("/ProfessionalReferral/LocalOfferDetail", new { serviceId });
        }
        ServiceId = model.ServiceId;
        ServiceName = model.ServiceName;    
        if (!string.IsNullOrEmpty(model.ReasonForSupport))
            TextAreaValue = model.ReasonForSupport;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(TextAreaValue) || TextAreaValue.Length > 500)
        {
            ValidationValid = false;
            return Page();
        }

        var model = await _referralDistributedCache.GetProfessionalReferralAsync();
        if (model == null)
        {
            // session has expired and we don't have a model to work with
            // likely the user has come back to this page after a long time
            // send them back to the start of the journey
            return RedirectToPage("/ProfessionalReferral/LocalOfferDetail", new { ServiceId });
        }
        model.ReasonForSupport = TextAreaValue;
        await _referralDistributedCache.SetProfessionalReferralAsync(model);

        return RedirectToPage("/ProfessionalReferral/ContactDetails", new
        {
            ServiceId,
            ServiceName
        });
    }
}
