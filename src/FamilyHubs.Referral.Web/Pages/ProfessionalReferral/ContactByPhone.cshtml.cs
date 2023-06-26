using System.ComponentModel.DataAnnotations;
using System.Web;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.ValidationAttributes;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ContactByPhoneModel : ProfessionalReferralCacheModel
{
    [BindProperty]
    public string? TelephoneNumber { get; set; }

    [BindProperty]
    public ReferrerContactType? Contact { get; set; }

    public string? ErrorMessage { get; set; }

    public ContactByPhoneModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
        : base(ConnectJourneyPage.ContactByPhone, connectionRequestDistributedCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        if (!HasErrors)
        {
            if (model.ReferrerContact != null)
            {
                Contact = model.ReferrerContact;
                TelephoneNumber = model.TelephoneNumber;
            }

            return;
        }

        if (model.ErrorState!.Errors.Contains(ProfessionalReferralError.ContactByPhone_NoContactSelected))
        {
            ErrorMessage = "Select how the service can contact you";
            return;
        }
        if (model.ErrorState!.Errors.Contains(ProfessionalReferralError.ContactByPhone_NoTelephoneNumber))
        {
            ErrorMessage = "Enter a UK telephone number";
            return;
        }
        if (model.ErrorState!.Errors.Contains(ProfessionalReferralError.ContactByPhone_InvalidTelephoneNumber))
        {
            ErrorMessage = "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192";
        }
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        ProfessionalReferralError? error = null;
        if (Contact == null)
        {
            error = ProfessionalReferralError.ContactByPhone_NoContactSelected;
        }
        else if (Contact == ReferrerContactType.TelephoneAndEmail)
        {
            if (string.IsNullOrEmpty(TelephoneNumber))
            {
                error = ProfessionalReferralError.ContactByPhone_NoTelephoneNumber;
            }
            else if (UkGdsTelephoneNumberAttribute.IsValid(TelephoneNumber) != ValidationResult.Success)
            {
                error = ProfessionalReferralError.ContactByPhone_InvalidTelephoneNumber;
            }
        }

        if (error != null)
        {
            return RedirectToSelf(null, error.Value);
        }

        model.ReferrerContact = Contact;
        model.TelephoneNumber = TelephoneNumber;

        return NextPage();
    }
}