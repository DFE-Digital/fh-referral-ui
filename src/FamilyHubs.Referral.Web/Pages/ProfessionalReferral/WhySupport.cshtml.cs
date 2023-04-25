using FamilyHubs.Referral.Core.DistributedCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public enum TextAreaValidation
{
    Valid,
    Empty,
    TooLong
}

public class WhySupportModel : PageModel
{
    private readonly IConnectionRequestDistributedCache _connectionRequestDistributedCache;

    [BindProperty]
    public string? ServiceId { get; set; }
    [BindProperty]
    public string? ServiceName { get; set; }

    [BindProperty]
    public string? TextAreaValue { get; set; }

    public TextAreaValidation TextAreaValidation { get; set; } = TextAreaValidation.Valid;

    public WhySupportModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
    {
        _connectionRequestDistributedCache = connectionRequestDistributedCache;
    }

    public async Task<IActionResult> OnGetAsync(string serviceId)
    {
        var model = await _connectionRequestDistributedCache.GetAsync();
        if (model == null)
        {
            // session has expired and we don't have a model to work with
            // likely the user has come back to this page after a long time
            // send them back to the start of the journey
            return RedirectToPage("/ProfessionalReferral/LocalOfferDetail", new { serviceId });
        }
        ServiceId = model.ServiceId;
        ServiceName = model.ServiceName;    
        if (!string.IsNullOrEmpty(model.Reason))
            TextAreaValue = model.Reason;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(TextAreaValue))
        {
            TextAreaValidation = TextAreaValidation.Empty;
            return Page();
        }

        if (TextAreaValue.Length > 500)
        {
            TextAreaValidation = TextAreaValidation.TooLong;
            return Page();
        }

        var model = await _connectionRequestDistributedCache.GetAsync();
        if (model == null)
        {
            // session has expired and we don't have a model to work with
            // likely the user has come back to this page after a long time
            // send them back to the start of the journey
            return RedirectToPage("/ProfessionalReferral/LocalOfferDetail", new { ServiceId });
        }
        model.Reason = TextAreaValue;
        await _connectionRequestDistributedCache.SetAsync(model);

        return RedirectToPage("/ProfessionalReferral/ContactDetails", new
        {
            ServiceId,
            ServiceName
        });
    }
}