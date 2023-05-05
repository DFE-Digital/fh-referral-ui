using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.Shared;

//todo: rename the 2 base classes
//todo: make other base class derive from this one?
public class ProfessionalReferralNoSessionModel : PageModel
{
    public string? ServiceId { get; set; }
    public string? ServiceName { get; set; }

    protected virtual Task<IActionResult> OnSafeGetAsync(string serviceId, string serviceName)
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

        return await OnSafeGetAsync(serviceId, serviceName);
    }
}