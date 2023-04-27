using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ContactDetailsModel : ProfessionalReferralModel
{
    public bool ValidationValid { get; private set; } = true;
    public string? FullName { get; set; }

    [BindProperty]
    public string? ServiceName { get; set; }

    [BindProperty]
    public string? Email { get; set; }

    [BindProperty]
    public string? Telephone { get; set; }

    [BindProperty]
    public string? Textphone { get; set; }

    [BindProperty]
    public string? Letter { get; set; }

    public ContactDetailsModel(IConnectionRequestDistributedCache connectionRequestCache) : base(connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        //todo: don't pass name through, once in cache
        ServiceName = model.ServiceName;

        FullName = model.FamilyContactFullName;
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
        
        var model = await ConnectionRequestCache.GetAsync();
        //todo: handle missing model
        model!.EmailSelected = !string.IsNullOrEmpty(Email);
        model.TelephoneSelected = !string.IsNullOrEmpty(Telephone);
        model.TextPhoneSelected = !string.IsNullOrEmpty(Textphone);
        model.LetterSelected = !string.IsNullOrEmpty(Letter);
        await ConnectionRequestCache.SetAsync(model);

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
