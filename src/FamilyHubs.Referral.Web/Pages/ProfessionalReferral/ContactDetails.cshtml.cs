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
    public bool Email { get; set; }

    [BindProperty]
    public bool Telephone { get; set; }

    [BindProperty]
    public bool Textphone { get; set; }

    [BindProperty]
    public bool Letter { get; set; }

    public ContactDetailsModel(IConnectionRequestDistributedCache connectionRequestCache) : base(connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        FullName = model.FamilyContactFullName;
        Email = model.EmailSelected;
        Telephone = model.TelephoneSelected;
        Textphone = model.TextPhoneSelected;
        Letter = model.LetterSelected;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || (!Email && !Telephone && !Textphone && !Letter))
        {
            ValidationValid = false;
            
            return Page();
        }
        
        var model = await ConnectionRequestCache.GetAsync();
        //todo: handle missing model
        model!.EmailSelected = Email;
        model.TelephoneSelected = Telephone;
        model.TextPhoneSelected = Textphone;
        model.LetterSelected = Letter;
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
