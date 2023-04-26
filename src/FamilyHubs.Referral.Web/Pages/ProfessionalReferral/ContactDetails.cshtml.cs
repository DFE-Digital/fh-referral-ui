using FamilyHubs.Referral.Core.DistributedCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ContactDetailsModel : PageModel
{
    private readonly IConnectionRequestDistributedCache _connectionRequestDistributedCache;
    public bool ValidationValid { get; private set; } = true;
    public string? FullName { get; set; }

    [BindProperty]
    public string? Email { get; set; }

    [BindProperty]
    public string? Telephone { get; set; }

    [BindProperty]
    public string? Textphone { get; set; }

    [BindProperty]
    public string? Letter { get; set; }

    public ContactDetailsModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
    {
        _connectionRequestDistributedCache = connectionRequestDistributedCache;
    }

    public async Task OnGetAsync()
    {
        var model = await _connectionRequestDistributedCache.GetAsync();
        //todo: handle missing model. have base class for all pages that handles this?

        //todo: why default to "Family"?
        //FullName = !string.IsNullOrEmpty(model.FullName) ? model.FullName : "Family";
        FullName = model!.FamilyContactFullName;
        Email = (model.EmailSelected) ? "Email" : null;
        Telephone = (model.TelephoneSelected) ? "Telephone" : null;
        Textphone = (model.TextPhoneSelected) ? "Textphone" : null;
        Letter = (model.LetterSelected) ? "Letter" : null;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Telephone) && string.IsNullOrEmpty(Textphone) && string.IsNullOrEmpty(Letter)))
        {
            ValidationValid = false;
            
            return Page();
        }
        
        var model = await _connectionRequestDistributedCache.GetAsync();
        //todo: handle missing model
        model!.EmailSelected = !string.IsNullOrEmpty(Email);
        model.TelephoneSelected = !string.IsNullOrEmpty(Telephone);
        model.TextPhoneSelected = !string.IsNullOrEmpty(Textphone);
        model.LetterSelected = !string.IsNullOrEmpty(Letter);
        await _connectionRequestDistributedCache.SetAsync(model);

        string destination = string.Empty;
        if (model.EmailSelected)
        {
            destination = "Email";
        }
        else if (model.TelephoneSelected) 
        {
            destination = "Telephone";
        }
        else if (model.TextPhoneSelected)
        {
            destination = "Textphone";
        }
        else if (model.LetterSelected)
        {
            destination = "Letter";
        }

        return RedirectToPage($"/ProfessionalReferral/{destination}", new
        {
        });
    }
}
