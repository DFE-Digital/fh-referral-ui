using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
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

    //todo: work these into base
    public IEnumerable<int> ErrorIds
    {
        get
        {
            return ConnectionRequestModel?.ErrorState?.Errors.Select(e => (int) e) ?? Enumerable.Empty<int>();
        }
    }

    public Error GetError(int errorId)
    {
        //todo: have a static immutable dictionary
        return (ProfessionalReferralError) errorId switch
        {
            ProfessionalReferralError.ContactByPhone_NoContactSelected
                => new Error((int)ProfessionalReferralError.ContactByPhone_NoContactSelected, "email", "Select how the service can contact you"),
            ProfessionalReferralError.ContactByPhone_NoTelephoneNumber
                => new Error((int)ProfessionalReferralError.ContactByPhone_NoTelephoneNumber, "contact-by-phone", "Enter a UK telephone number"),
            ProfessionalReferralError.ContactByPhone_InvalidTelephoneNumber
                => new Error((int)ProfessionalReferralError.ContactByPhone_InvalidTelephoneNumber, "contact-by-phone", "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192"),
            _ => throw new NotImplementedException()
        };
    }

    //public bool HasError(int errorId)
    //{
    //    return ErrorIds.Contains(errorId);
    //}

    public bool HasError(params int[] errorIds)
    {
        return GetErrorIdIfTriggered(errorIds) != null;
//#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
//        foreach (int errorId in errorIds)
//        {
//            if (ErrorIds.Contains(errorId))
//            {
//                return true;
//            }
//        }
//#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions

//        return false;
    }

    [SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "LINQ expression version is less simple")]
    public int? GetErrorIdIfTriggered(params int[] mutuallyExclusiveErrorIds)
    {
        if (!mutuallyExclusiveErrorIds.Any())
        {
            return ErrorIds.Any() ? ErrorIds.First() : null;
        }

        foreach (int errorId in mutuallyExclusiveErrorIds)
        {
            if (ErrorIds.Contains(errorId))
            {
                return errorId;
            }
        }

        return null;
    }

    public Error? GetErrorIfTriggered(params int[] mutuallyExclusiveErrorIds)
    {
        int? currentErrorId = GetErrorIdIfTriggered(mutuallyExclusiveErrorIds);
        return currentErrorId != null ? GetError(currentErrorId.Value) : null;
    }
}