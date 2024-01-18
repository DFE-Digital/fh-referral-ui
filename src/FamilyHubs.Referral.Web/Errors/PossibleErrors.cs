using FamilyHubs.Referral.Core.Models;
using FamilyHubs.SharedKernel.Razor.ErrorNext;
using System.Collections.Immutable;

namespace FamilyHubs.Referral.Web.Errors;

public static class PossibleErrors
{
    public static readonly ImmutableDictionary<int, PossibleError> All = ImmutableDictionary
        .Create<int, PossibleError>()
        .Add(ErrorId.Consent_NoConsentSelected, "Select whether you have permission to share details")
        .Add(ErrorId.SharePrivacy_NoSelection, "Select whether you have shared our privacy statement")
        .Add(ErrorId.ContactByPhone_NoContactSelected, "Select how the service can contact you")
        .Add(ErrorId.ContactByPhone_NoTelephoneNumber, "Enter a UK telephone number")
        .Add(ErrorId.ContactByPhone_InvalidTelephoneNumber, "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192")
        .Add(ErrorId.WhySupport_NothingEntered, "Enter details about the people who need support")
        .Add(ErrorId.WhySupport_TooLong, "Reason for the connection request must be 500 characters or less")
        .Add(ErrorId.ContactMethods_NothingEntered, "Enter details about the people who need support")
        .Add(ErrorId.ContactMethods_TooLong, "How the service can engage with the family must be 500 characters or less")
        .Add(ErrorId.ContactDetails_NoContactMethodsSelected, "ContactMethods_0_", "Select a contact method")
        .Add(ErrorId.ChangeName_EnterAName, "Enter a name")
        ;
}