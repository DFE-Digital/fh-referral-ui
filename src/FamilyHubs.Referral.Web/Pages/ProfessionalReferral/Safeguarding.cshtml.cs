using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SafeguardingModel : PageModel
{
    public string? ServiceId { get; set; }
    public string? ServiceName { get; set; }

    public void OnGet(string serviceId, string serviceName)
    {
        ServiceId = serviceId;
        ServiceName = serviceName;
    }
}
