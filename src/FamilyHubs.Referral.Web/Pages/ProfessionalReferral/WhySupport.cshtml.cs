using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public enum TextAreaValidation
{
    Valid,
    Empty,
    TooLong
}

public class WhySupportModel : ProfessionalReferralModel
{
    [BindProperty]
    public string? ServiceName { get; set; }

    [BindProperty]
    public string? TextAreaValue { get; set; }

    public TextAreaValidation TextAreaValidation { get; set; } = TextAreaValidation.Valid;

    public WhySupportModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        ServiceName = model.ServiceName;
        if (!string.IsNullOrEmpty(model.Reason))
            TextAreaValue = model.Reason;
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

        var model = await ConnectionRequestCache.GetAsync();
        if (model == null)
        {
            // session has expired and we don't have a model to work with
            // likely the user has come back to this page after a long time
            // send them back to the start of the journey
            return RedirectToPage("/ProfessionalReferral/LocalOfferDetail", new { ServiceId });
        }
        model.Reason = TextAreaValue;
        await ConnectionRequestCache.SetAsync(model);

        return RedirectToPage("/ProfessionalReferral/ContactDetails", new
        {
            ServiceId,
            ServiceName
        });
    }
}
