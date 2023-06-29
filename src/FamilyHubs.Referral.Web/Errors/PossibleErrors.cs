using FamilyHubs.Referral.Core.Models;
using System.Collections.Immutable;

namespace FamilyHubs.Referral.Web.Errors;

public static class PossibleErrors
{
    //todo: helper?
    public static readonly ImmutableDictionary<int, Error> All = ImmutableDictionary
        .Create<int, Error>()
        .Add((int)ErrorId.Consent_NoConsentSelected, new Error((int)ErrorId.Consent_NoConsentSelected, "consent-yes", "Select whether you have permission from the family to share details"))
        .Add((int)ErrorId.SharePrivacy_NoSelection, new Error((int)ErrorId.SharePrivacy_NoSelection, "shared-privacy-yes", "Select whether you have shared our privacy statement with the family"))
        .Add((int)ErrorId.ContactByPhone_NoContactSelected, new Error((int)ErrorId.ContactByPhone_NoContactSelected, "email", "Select how the service can contact you"))
        .Add((int)ErrorId.ContactByPhone_NoTelephoneNumber, new Error((int)ErrorId.ContactByPhone_NoTelephoneNumber, "contact-by-phone", "Enter a UK telephone number"))
        .Add((int)ErrorId.ContactByPhone_InvalidTelephoneNumber, new Error((int)ErrorId.ContactByPhone_InvalidTelephoneNumber, "contact-by-phone", "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192"))
        .Add((int)ErrorId.WhySupport_NothingEntered, new Error((int)ErrorId.WhySupport_NothingEntered, "reason", "Enter a reason for the connection request"))
        .Add((int)ErrorId.WhySupport_TooLong, new Error((int)ErrorId.WhySupport_TooLong, "reason", "Reason for the connection request must be 500 characters or less"))
        .Add((int)ErrorId.ContactMethods_NothingEntered, new Error((int)ErrorId.ContactMethods_NothingEntered, "reason", "Enter how best to engage with this family"))
        .Add((int)ErrorId.ContactMethods_TooLong, new Error((int)ErrorId.ContactMethods_TooLong, "reason", "How the service can engage with the family must be 500 characters or less"))
        ;
}