using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SafeguardingModel : PageModel
{
    public string BackUrl { get; set; } = default!;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;
    public void OnGet(string id, string name)
    {
        Id = id;
        Name = name;

        string encodeName = Uri.EscapeDataString(name);
        BackUrl = $"/ProfessionalReferral/Consent?id={id}&name={encodeName}";
    }
}
