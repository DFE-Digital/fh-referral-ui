using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SafeguardingModel : PageModel
{
    public string BackUrl { get; set; } = default!;

    [BindProperty]
    public string ServiceId { get; set; } = default!;
    [BindProperty]
    public string ServiceName { get; set; } = default!;
    public void OnGet(string serviceId, string serviceName)
    {
        ServiceId = serviceId;
        ServiceName = serviceName;
    }
}
