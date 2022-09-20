using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class ContactDetailsModel : PageModel
{
    [BindProperty]
    public string FullName { get; set; } = default!;

    [BindProperty]
    public string HasSpecialNeeds { get; set; } = default!;

    [BindProperty]
    public string Email { get; set; } = default!;

    [BindProperty]
    public string Phone { get; set; } = default!;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;
    public void OnGet(string id, string name, string fullName, string hasSpecialNeeds, string email, string phone)
    {
        Id = id;
        Name = name;
        FullName = fullName;
        HasSpecialNeeds = hasSpecialNeeds;

        if (!string.IsNullOrEmpty(email))
            Email = email;
        if (!string.IsNullOrEmpty(phone))
            Phone = phone;
    }

    public IActionResult OnPost()
    {
        return RedirectToPage("/ProfessionalReferral/WhySupport", new
        {
            id = Id,
            name = Name,
            fullName = FullName,
            hasSpecialNeeds = HasSpecialNeeds,
            email = Email,
            phone = Phone
        });
    }
}
