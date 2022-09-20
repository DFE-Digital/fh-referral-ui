using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class SafeguardingModel : PageModel
{
    [BindProperty]
    public string IsImmediateHarm { get; set; } = default!;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;
    public void OnGet(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public IActionResult OnPost(string id, string name)
    {
        if (string.Compare(IsImmediateHarm, "yes", StringComparison.OrdinalIgnoreCase) == 0)
        {
            return RedirectToPage("/ProfessionalReferral/Consent", new
            {
                id = id,
                name = name
            });
        }

        return RedirectToPage("/ProfessionalReferral/SafeguardingShutter", new
        {
        });

    }
}
