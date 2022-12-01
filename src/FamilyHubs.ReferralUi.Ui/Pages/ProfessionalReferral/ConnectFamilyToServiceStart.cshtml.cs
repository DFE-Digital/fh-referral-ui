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
    public void OnGet(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public IActionResult OnPost(string id, string name)
    {
        return RedirectToPage("/ProfessionalReferral/SignIn", new
        {
            id = id,
            name = name
        });

    }
}
