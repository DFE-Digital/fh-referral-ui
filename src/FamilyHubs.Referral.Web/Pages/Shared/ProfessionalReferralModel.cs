using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.Shared;

public abstract class ProfessionalReferralModel : PageModel
{
    [BindProperty]
    public string? ServiceId { get; set; }

    protected readonly IConnectionRequestDistributedCache ConnectionRequestCache;

    protected ProfessionalReferralModel(IConnectionRequestDistributedCache connectionRequestCache)
    {
        ConnectionRequestCache = connectionRequestCache;
    }

    protected abstract void OnGetWithModel(ConnectionRequestModel model);

    public async Task<IActionResult> OnGetAsync(string serviceId)
    {
        var model = await ConnectionRequestCache.GetAsync();
        if (model == null)
        {
            // session has expired and we don't have a model to work with
            // likely the user has come back to this page after a long time
            // send them back to the start of the journey
            return RedirectToPage("/ProfessionalReferral/LocalOfferDetail", new {serviceId});
        }

        ServiceId = model.ServiceId;

        OnGetWithModel(model);

        return Page();
    }
}