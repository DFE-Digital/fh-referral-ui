using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class PersonalDetailsModel : PageModel
{
    [BindProperty]
    public string FullName { get; set; } = default!;

    [BindProperty]
    public string HasSpecialNeeds { get; set; } = default!;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    [BindProperty]
    public bool NeedsValid { get; set; } = true;

    public void OnGet(string id, string name, string fullName, string hasSpecialNeeds)
    {
        Id = id;
        Name = name;

        if (!string.IsNullOrEmpty(fullName))
            FullName = fullName;
        if (!string.IsNullOrEmpty(hasSpecialNeeds))
            HasSpecialNeeds = hasSpecialNeeds;
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            if (HasSpecialNeeds == null)
                NeedsValid = false;

            if (FullName == null || FullName.Trim().Length == 0 || FullName.Length > 255)
                ValidationValid = false;

            return Page();
        }
            
        return RedirectToPage("/ProfessionalReferral/ContactDetails", new
        {
            id = Id,
            name = Name,
            fullName = FullName,
            hasSpecialNeeds = HasSpecialNeeds
        });

    }
    
}
