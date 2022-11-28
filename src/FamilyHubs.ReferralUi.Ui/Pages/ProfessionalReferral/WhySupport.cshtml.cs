using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class WhySupportModel : PageModel
{
    [BindProperty]
    public string FullName { get; set; } = default!;

    [BindProperty]
    public string Email { get; set; } = default!;

    [BindProperty]
    public string Telephone { get; set; } = default!;

    [BindProperty]
    public string Textphone { get; set; } = default!;

    [BindProperty]
    public string ReasonForSupport { get; set; } = default!;

    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public void OnGet(string id, string name, string fullName, string email, string telephone, string textphone, string reasonForSupport)
    {
        Id = id;
        Name = name;
        FullName = fullName;
        Email = email;
        Telephone = telephone;
        Textphone = textphone;

        if (!string.IsNullOrEmpty(reasonForSupport))
            ReasonForSupport = reasonForSupport;
    }

    public IActionResult OnPost()
    {
        ModelState.Remove("Email");
        ModelState.Remove("Telephone");
        ModelState.Remove("Textphone");

        if (!ModelState.IsValid || (ReasonForSupport == null || ReasonForSupport.Trim().Length == 0 || ReasonForSupport.Length > 500))
        {
            ValidationValid = false;
            return Page();
        }

        return RedirectToPage("/ProfessionalReferral/CheckReferralDetails", new
        {
            id = Id,
            name = Name,
            fullName = FullName,
            email = Email,
            telephone = Telephone,
            textphone = Textphone,
            reasonForSupport = ReasonForSupport
        });

    }
}
