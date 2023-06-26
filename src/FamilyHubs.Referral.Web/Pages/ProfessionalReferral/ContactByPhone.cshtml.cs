using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.ValidationAttributes;
using FamilyHubs.Referral.Web.Errors;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ContactByPhoneModel : ProfessionalReferralCacheModel, IErrorSummary
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
        if (!HasErrors && model.ReferrerContact != null)
        {
            Contact = model.ReferrerContact;
            TelephoneNumber = model.TelephoneNumber;
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

    public IEnumerable<int> ErrorIds
    {
        get
        {
            return ConnectionRequestModel?.ErrorState?.Errors.Select(e => (int) e) ?? Enumerable.Empty<int>();
        }
    }

    public Error GetError(int errorId)
    {
        return (ProfessionalReferralError) errorId switch
        {
            ProfessionalReferralError.ContactByPhone_NoContactSelected
                => new Error("email", "Select how the service can contact you"),
            ProfessionalReferralError.ContactByPhone_NoTelephoneNumber
                => new Error("contact-by-phone", "Enter a UK telephone number"),
            ProfessionalReferralError.ContactByPhone_InvalidTelephoneNumber
                => new Error("contact-by-phone", "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192"),
            _ => throw new NotImplementedException()
        };
    }
}