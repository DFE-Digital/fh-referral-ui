using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public partial class ContactDetailsModel : PageModel
{
    private readonly IRedisCacheService _cacheService;

    [BindProperty]
    public List<string> ContactSelection { get; set; } = new List<string>();

    [BindProperty]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string? Email { get; set; }

    [BindProperty]
    public bool EmailValid { get; set; } = true;

    [BindProperty]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string? Telephone { get; set; }

    [BindProperty]
    public bool TelephoneValid { get; set; } = true;

    public string FullName { get; set; } = string.Empty;

    [BindProperty]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string? Textphone { get; set; }

    [BindProperty]
    public bool TextphoneValid { get; set; } = true;

    [BindProperty]
    public bool ValidationValid { get; set; } = true;

    public ContactDetailsModel(IRedisCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public void OnGet()
    {
        string userKey = _cacheService.GetUserKey();
        ConnectWizzardViewModel model = _cacheService.RetrieveConnectWizzardViewModel(userKey);
        FullName= model.FullName;
        
        if (!string.IsNullOrEmpty(model.EmailAddress))
        {
            Email = model.EmailAddress;
            ContactSelection.Add("email");
        } 
        if (!string.IsNullOrEmpty(model.Telephone))
        {
            Telephone = model.Telephone;
            ContactSelection.Add("telephone");
        }   
        if (!string.IsNullOrEmpty(model.Textphone))
        {
            Textphone = model.Textphone;
            ContactSelection.Add("textphone");
        }
            
    }

    public IActionResult OnPost()
    {
        SetDefaultContactSelection();

        string userKey = _cacheService.GetUserKey();
        ConnectWizzardViewModel model = _cacheService.RetrieveConnectWizzardViewModel(userKey);
        FullName = model.FullName;

        if (ContactSelection == null || !ContactSelection.Any())
        {
            ValidationValid = false;
            ModelState.AddModelError("Select One Option", "Please select one option");
            return Page();
        }

        if (ContactSelection != null)
        {
            if (!ContactSelection.Contains("email") && !ContactSelection.Contains("telephone") && !ContactSelection.Contains("textphone"))
            {
                ValidationValid = false;
                ModelState.AddModelError("Select One Option", "Please select one option");
                return Page();
            }

            CheckEmailContactSelection();
            CheckTelephoneContactSelection();
            CheckTextphoneContactSelection();
        }

        
        if (!ModelState.IsValid || !ValidationValid)
        {
            ValidationValid = false;
            return Page();
        }

        model.EmailAddress = Email;
        model.Telephone = Telephone;
        model.Textphone = Textphone;

        _cacheService.StoreConnectWizzardViewModel(userKey, model);

        return RedirectToPage("/ProfessionalReferral/WhySupport", new
        {
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
                ModelState.AddModelError("telephone", "Telephone is invalid (can not contain spaces)");
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
