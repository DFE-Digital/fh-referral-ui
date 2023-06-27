using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.ValidationAttributes;
using FamilyHubs.Referral.Web.Errors;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ContactByPhoneModel : ProfessionalReferralCacheModel
{
    // one of these for all errors?
    private static readonly ImmutableDictionary<int, Error> PossibleErrors = ImmutableDictionary
        .Create<int, Error>()
        .Add((int)ProfessionalReferralError.ContactByPhone_NoContactSelected, new Error((int)ProfessionalReferralError.ContactByPhone_NoContactSelected, "email", "Select how the service can contact you"))
        .Add((int)ProfessionalReferralError.ContactByPhone_NoTelephoneNumber, new Error((int)ProfessionalReferralError.ContactByPhone_NoTelephoneNumber, "contact-by-phone", "Enter a UK telephone number"))
        .Add((int)ProfessionalReferralError.ContactByPhone_InvalidTelephoneNumber, new Error((int)ProfessionalReferralError.ContactByPhone_InvalidTelephoneNumber, "contact-by-phone", "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192"));

    public ErrorState? ErrorState { get; private set; }

    [BindProperty]
    public string? TelephoneNumber { get; set; }

    [BindProperty]
    public ReferrerContactType? Contact { get; set; }

    public ContactByPhoneModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
        : base(ConnectJourneyPage.ContactByPhone, connectionRequestDistributedCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        if (HasErrors)
        {
            ErrorState = new ErrorState(PossibleErrors,
                ConnectionRequestModel?.ErrorState?.Errors.Select(e => (int) e) ?? Enumerable.Empty<int>());
        }
        else if (model.ReferrerContact != null)
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
        model.ReferrerTelephone = Contact == ReferrerContactType.TelephoneAndEmail ? TelephoneNumber : null;

        return NextPage();
    }
}