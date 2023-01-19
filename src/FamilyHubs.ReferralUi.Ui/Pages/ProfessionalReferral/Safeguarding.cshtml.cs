using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class SafeguardingModel : PageModel
{
    public string ReferralId { get; set; } = default!;

    [BindProperty]
    public string IsImmediateHarm { get; set; } = default!;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;
    public void OnGet(string id, string name, string referralId)
    {
        Id = id;
        Name = name;
        ReferralId = referralId;
    }

    public IActionResult OnPost(string id, string name, string referralId)
    {
        ModelState.Remove("ReferralId");

        if (!ModelState.IsValid || IsImmediateHarm == null)
        {
            Id = id;
            Name = name;
            ReferralId = referralId;
            ValidationValid = false;
            return Page();
        }

        if (string.Compare(IsImmediateHarm, "no", StringComparison.OrdinalIgnoreCase) == 0)
        {
            return RedirectToPage("/ProfessionalReferral/Consent", new
            {
                id = id,
                name = name,
                referralId = referralId
            });
        }

        return RedirectToPage("/ProfessionalReferral/SafeguardingShutter", new
        {
        });

    }
}
