using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.Shared;

public enum JourneyFlow
{
    Normal,
    ChangingPage,
    ChangingContactMethods
}

public class ProfessionalReferralModel : PageModel
{
    public string? ServiceId { get; set; }
    public JourneyFlow Flow { get; set; }

    protected virtual Task<IActionResult> OnSafeGetAsync()
    {
        return Task.FromResult((IActionResult)Page());
    }

    protected virtual Task<IActionResult> OnSafePostAsync()
    {
        return Task.FromResult((IActionResult)Page());
    }

    public async Task<IActionResult> OnGetAsync(string serviceId, string? changing = null)
    {
        if (serviceId == null)
        {
            // someone's been monkeying with the query string and we don't have the service details we need
            // we can't send them back to the start of the journey because we don't know what service they were looking at
            // so we'll just send them to the home page
            return RedirectToPage("/Index");
        }

        ServiceId = serviceId;

        //todo: move page next/previous into here and handle all journey pages here, or here and derived
        Flow = changing switch
        {
            "page" => JourneyFlow.ChangingPage,
            "contact-methods" => JourneyFlow.ChangingContactMethods,
            _ => JourneyFlow.Normal
        };

        return await OnSafeGetAsync();
    }

    public async Task<IActionResult> OnPostAsync(string serviceId)
    {
        ServiceId = serviceId;

        return await OnSafePostAsync();
    }

    protected IActionResult RedirectToProfessionalReferralPage(string page)
    {
        return RedirectToPage($"/ProfessionalReferral/{page}", new
        {
            ServiceId
        });
    }
}