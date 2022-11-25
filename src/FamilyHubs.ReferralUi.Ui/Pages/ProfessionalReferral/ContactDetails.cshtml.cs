using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class ContactDetailsModel : PageModel
{
    [BindProperty]
    public string FullName { get; set; } = default!;

    [BindProperty]
    public List<string> ContactSelection { get; set; } = new List<string>();


    [BindProperty]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string? Email { get; set; } = default!;

    [BindProperty]
    public bool EmailValid { get; set; } = true;

    [BindProperty]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string? Telephone { get; set; } = default!;

    [BindProperty]
    public bool TelephoneValid { get; set; } = true;

    [BindProperty]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string? Textphone { get; set; } = default!;

    [BindProperty]
    public bool TextphoneValid { get; set; } = true;


    [BindProperty]
    public string Id { get; set; } = default!;
    [BindProperty]
    public string Name { get; set; } = default!;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public void OnGet(string id, string name, string fullName, string email, string telephone, string textphone)
    {
        Id = id;
        Name = name;
        FullName = fullName;

        if (!string.IsNullOrEmpty(email))
        {
            Email = email;
            ContactSelection.Add("email");
        } 
        if (!string.IsNullOrEmpty(telephone))
        {
            Telephone = telephone;
            ContactSelection.Add("telephone");
        }   
        if (!string.IsNullOrEmpty(textphone))
        {
            Textphone = textphone;
            ContactSelection.Add("textphone");
        }
            
    }

    public IActionResult OnPost()
    {
        if (ContactSelection == null || !ContactSelection.Contains("email"))
        {
            this.Email = String.Empty;
        }
        if (ContactSelection == null || !ContactSelection.Contains("telephone"))
        {
            this.Telephone = String.Empty;
        }
        if (ContactSelection == null || !ContactSelection.Contains("textphone"))
        {
            this.Textphone = String.Empty;
        }

        if (ContactSelection == null || !ContactSelection.Any())
        {
            ValidationValid = false;
            ModelState.AddModelError("Select One Option", "Please select one option");
            return Page();
        }

        if (ContactSelection != null)
        {
            if (!ContactSelection.Contains("email") && !ContactSelection.Contains("phone") && !ContactSelection.Contains("website") && !ContactSelection.Contains("textphone"))
            {
                ValidationValid = false;
                ModelState.AddModelError("Select One Option", "Please select one option");
                return Page();
            }

            if (ContactSelection.Contains("email"))
            {
                if (string.IsNullOrWhiteSpace(Email))
                {
                    EmailValid = false;
                    ValidationValid = false;
                }
                else if (!Regex.IsMatch(Email.ToString(), @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
                {
                    EmailValid = false;
                    ValidationValid = false;
                }
            }

            if (ContactSelection.Contains("telephone"))
            {
                if (string.IsNullOrWhiteSpace(Telephone))
                {
                    TelephoneValid = false;
                    ValidationValid = false;
                }
                else if (!Regex.IsMatch(Telephone.ToString(), @"^[A-Za-z0-9]*$"))
                {
                    TelephoneValid = false;
                    ValidationValid = false;
                }

            }

            

            if (ContactSelection.Contains("textphone"))
            {
                if (string.IsNullOrWhiteSpace(Textphone))
                {
                    TextphoneValid = false;
                    ValidationValid = false;
                }
                else if (!Regex.IsMatch(Textphone.ToString(), @"^[A-Za-z0-9]*$"))
                {
                    TextphoneValid = false;
                    ValidationValid = false;
                }

            }
        }

        if (!ModelState.IsValid)
        {
            ValidationValid = false;
            return Page();
        }

        return RedirectToPage("/ProfessionalReferral/WhySupport", new
        {
            id = Id,
            name = Name,
            fullName = FullName,
            email = Email,
            telephone = Telephone,
            textphone = Textphone,
        });
    }
}
