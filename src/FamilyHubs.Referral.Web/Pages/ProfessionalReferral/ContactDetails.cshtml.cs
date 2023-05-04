using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ContactDetailsModel : ProfessionalReferralModel
{
    //todo: move into base?
    public bool ValidationValid { get; private set; } = true;
    public string? FullName { get; set; }

    [BindProperty]
    public bool[] ContactMethods { get; set; } = new bool[(int)ContactMethod.Last+1];

    public ContactDetailsModel(IConnectionRequestDistributedCache connectionRequestCache) : base(connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        FullName = model.FamilyContactFullName;
        ContactMethods = model.ContactMethodsSelected;
    }

    protected override string? OnPostWithModel(ConnectionRequestModel model)
    {
        if (!(ModelState.IsValid && ContactMethods.Any(m => m)))
        {
            ValidationValid = false;
            return null;
        }

        model.ContactMethodsSelected = ContactMethods;

        return FirstContactMethodPage(model.ContactMethodsSelected);
    }
}
