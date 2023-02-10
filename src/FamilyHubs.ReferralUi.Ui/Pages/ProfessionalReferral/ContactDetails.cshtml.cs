using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public partial class ContactDetailsModel : PageModel
{
    [BindProperty]
    public string ReferralId { get; set; } = default!;
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

    public void OnGet(string id, string name, string fullName, string email, string telephone, string textphone, string referralId)
    {
        Id = id;
        Name = name;
        FullName = fullName;
        Email = email;
        Telephone = telephone;
        Textphone = textphone;
        ReferralId = referralId;

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
        SetDefaultContactSelection();

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

            CheckEmailContactSelection();
            CheckTelephoneContactSelection();
            CheckTextphoneContactSelection();
        }

        ModelState.Remove("ReferralId");

        if (!ModelState.IsValid || !ValidationValid)
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
            referralId = ReferralId,
        });
    }

    private void SetDefaultContactSelection()
    {
        if (ContactSelection == null || !ContactSelection.Contains("email"))
        {
            Email = string.Empty;
        }
        if (ContactSelection == null || !ContactSelection.Contains("telephone"))
        {
            Telephone = string.Empty;
        }
        if (ContactSelection == null || !ContactSelection.Contains("textphone"))
        {
            Textphone = string.Empty;
        }
    }

    private void CheckEmailContactSelection()
    {
        if (ContactSelection.Contains("email") && (string.IsNullOrWhiteSpace(Email) || !EmailRegex().IsMatch(Email.ToString())))
        {
            EmailValid = false;
            ValidationValid = false;
        }
    }

    private void CheckTelephoneContactSelection()
    {
        if (ContactSelection.Contains("telephone"))
        {
            if (string.IsNullOrWhiteSpace(Telephone))
            {
                TelephoneValid = false;
                ValidationValid = false;
            }
            else if (!PhoneRegex().IsMatch(Telephone.ToString()))
            {
                TelephoneValid = false;
                ValidationValid = false;
                ModelState.AddModelError("textphone", "Telephone is invalid (can not contain spaces)");
            }

        }
    }

    private void CheckTextphoneContactSelection()
    {
        if (ContactSelection.Contains("textphone"))
        {
            if (string.IsNullOrWhiteSpace(Textphone))
            {
                TextphoneValid = false;
                ValidationValid = false;
            }
            else if (!PhoneRegex().IsMatch(Textphone.ToString()))
            {
                TextphoneValid = false;
                ValidationValid = false;
                ModelState.AddModelError("textphone", "Textphone is invalid (can not contain spaces)");
            }
        }
    }

    [GeneratedRegex("^[A-Za-z0-9]*$")]
    private static partial Regex PhoneRegex();
    [GeneratedRegex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$")]
    private static partial Regex EmailRegex();
}
