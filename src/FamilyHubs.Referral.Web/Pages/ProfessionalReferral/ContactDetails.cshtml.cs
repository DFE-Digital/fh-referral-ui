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
        //todo: use array, if asp-for & binding works
        Email = model.ContactMethodsSelected[(int)ContactMethod.Email];
        Telephone = model.ContactMethodsSelected[(int)ContactMethod.Telephone];
        Textphone = model.ContactMethodsSelected[(int)ContactMethod.Textphone];
        Letter = model.ContactMethodsSelected[(int)ContactMethod.Letter];
    }

    protected override string? OnPostWithModel(ConnectionRequestModel model)
    {
        if (!(ModelState.IsValid && (Email || Telephone || Textphone || Letter)))
        {
            ValidationValid = false;
            return null;
        }

        model.ContactMethodsSelected[(int)ContactMethod.Email] = Email;
        model.ContactMethodsSelected[(int)ContactMethod.Telephone] = Telephone;
        model.ContactMethodsSelected[(int)ContactMethod.Textphone] = Textphone;
        model.ContactMethodsSelected[(int)ContactMethod.Letter] = Letter;

        return FirstContactMethodPage(model.ContactMethodsSelected);
    }
}
