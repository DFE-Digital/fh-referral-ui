using FamilyHubs.Referral.Core.Models;
using System.Collections.Immutable;
using FamilyHubs.SharedKernel.Razor.Errors;
using ErrorDictionary = System.Collections.Immutable.ImmutableDictionary<int, FamilyHubs.SharedKernel.Razor.Errors.Error>;

namespace FamilyHubs.Referral.Web.Errors;

public static class PossibleErrors
{
    //todo: use a tag helper for the error summary, and pass the htmlelementid to the tag helper (so that details of the view don't leak)
    public static readonly ErrorDictionary All = ImmutableDictionary
        .Create<int, Error>()
        .Add(ErrorId.Consent_NoConsentSelected, "consent-yes", "Select whether you have permission to share details")
        .Add(ErrorId.SharePrivacy_NoSelection, "shared-privacy-yes", "Select whether you have shared our privacy statement")
        .Add(ErrorId.ContactByPhone_NoContactSelected, "email", "Select how the service can contact you")
        .Add(ErrorId.ContactByPhone_NoTelephoneNumber, "contact-by-phone", "Enter a UK telephone number")
        .Add(ErrorId.ContactByPhone_InvalidTelephoneNumber, "contact-by-phone", "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192")
        .Add(ErrorId.WhySupport_NothingEntered, "reason", "Enter details about the people who need support")
        .Add(ErrorId.WhySupport_TooLong, "reason", "Reason for the connection request must be 500 characters or less")
        .Add(ErrorId.ContactMethods_NothingEntered, "reason", "Enter how best to engage with this family")
        .Add(ErrorId.ContactMethods_TooLong, "reason", "How the service can engage with the family must be 500 characters or less")
        .Add(ErrorId.ContactDetails_NoContactMethodsSelected, "ContactMethods_0_", "Select a contact method")
        .Add(ErrorId.ChangeName_EnterAName, "new-name", "Enter a name")
        ;
}