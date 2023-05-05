using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.Shared;

public class ProfessionalReferralModel : PageModel
{
    public string? ServiceId { get; set; }
    public string? ServiceName { get; set; }

    protected virtual Task<IActionResult> OnSafeGetAsync()
    {
        return Task.FromResult((IActionResult)Page());
    }

    protected virtual Task<IActionResult> OnSafePostAsync()
    {
        return Task.FromResult((IActionResult)Page());
    }

    public async Task<IActionResult> OnGetAsync(string serviceId, string serviceName)
    {
        if (serviceId == null || serviceName == null)
        {
            // someone's been monkeying with the query string and we don't have the service details we need
            // we can't send them back to the start of the journey because we don't know what service they were looking at
            // so we'll just send them to the home page
            return RedirectToPage("/Index");
        }

        ServiceId = serviceId;
        ServiceName = serviceName;

        return await OnSafeGetAsync();
    }

    public async Task<IActionResult> OnPostAsync(string serviceId, string serviceName)
    {
        ServiceId = serviceId;
        ServiceName = serviceName;

        return await OnSafePostAsync();
    }

    protected IActionResult RedirectToProfessionalReferralPage(string page)
    {
        return RedirectToPage($"/ProfessionalReferral/{page}", new
        {
            ServiceId,
            ServiceName
        });
    }
}