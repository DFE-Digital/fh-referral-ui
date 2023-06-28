using FamilyHubs.Referral.Core.Models;
using System.Collections.Immutable;

namespace FamilyHubs.Referral.Web.Errors;

public static class PossibleErrors
{
    public static readonly ImmutableDictionary<int, Error> All = ImmutableDictionary
        .Create<int, Error>()
        .Add((int)ErrorId.ContactByPhone_NoContactSelected, new Error((int)ErrorId.ContactByPhone_NoContactSelected, "email", "Select how the service can contact you"))
        .Add((int)ErrorId.ContactByPhone_NoTelephoneNumber, new Error((int)ErrorId.ContactByPhone_NoTelephoneNumber, "contact-by-phone", "Enter a UK telephone number"))
        .Add((int)ErrorId.ContactByPhone_InvalidTelephoneNumber, new Error((int)ErrorId.ContactByPhone_InvalidTelephoneNumber, "contact-by-phone", "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192"));
}