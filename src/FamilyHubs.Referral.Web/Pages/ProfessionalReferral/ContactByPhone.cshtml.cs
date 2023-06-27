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
        .Add((int)ErrorId.ContactByPhone_NoContactSelected, new Error((int)ErrorId.ContactByPhone_NoContactSelected, "email", "Select how the service can contact you"))
        .Add((int)ErrorId.ContactByPhone_NoTelephoneNumber, new Error((int)ErrorId.ContactByPhone_NoTelephoneNumber, "contact-by-phone", "Enter a UK telephone number"))
        .Add((int)ErrorId.ContactByPhone_InvalidTelephoneNumber, new Error((int)ErrorId.ContactByPhone_InvalidTelephoneNumber, "contact-by-phone", "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192"));

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
        ErrorState = ErrorState.Create(PossibleErrors, ConnectionRequestModel?.ErrorState?.Errors);
        if (!HasErrors && model.ReferrerContact != null)
        {
            Contact = model.ReferrerContact;
            TelephoneNumber = model.ReferrerTelephone;
        }
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        ErrorId? error = null;
        if (Contact == null)
        {
            error = ErrorId.ContactByPhone_NoContactSelected;
        }
        else if (Contact == ReferrerContactType.TelephoneAndEmail)
        {
            if (string.IsNullOrEmpty(TelephoneNumber))
            {
                error = ErrorId.ContactByPhone_NoTelephoneNumber;
            }
            else if (UkGdsTelephoneNumberAttribute.IsValid(TelephoneNumber) != ValidationResult.Success)
            {
                error = ErrorId.ContactByPhone_InvalidTelephoneNumber;
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