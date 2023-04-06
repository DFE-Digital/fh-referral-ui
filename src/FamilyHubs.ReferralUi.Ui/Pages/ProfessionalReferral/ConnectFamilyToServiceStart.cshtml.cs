using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class ConnectFamilyToServiceStartModel : PageModel
{
    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    public ConnectFamilyToServiceStartModel()
    {
    }
    public void OnGet(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public IActionResult OnPost(string id, string name)
    {
        return RedirectToPage("/ProfessionalReferral/Consent", new
        {
            id,
            name
        });
    }
}
