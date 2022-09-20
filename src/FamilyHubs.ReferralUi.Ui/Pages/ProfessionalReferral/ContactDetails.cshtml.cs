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
    public bool EmailValid { get; set; } = true;

    [BindProperty]
    public string Phone { get; set; } = default!;

    [BindProperty]
    public bool PhoneValid { get; set; } = true;


    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

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
        if (!ModelState.IsValid)
        {
            ValidationValid = false;
            if (Phone == null || Phone.Trim().Length == 0 || Phone.Length > 255)
                PhoneValid = false;

            if (Email == null || Email.Trim().Length == 0 || Email.Length > 15)
                EmailValid = false;

            return Page();
        }

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
