namespace FamilyHubs.Referral.Web.Models;

//todo: error enum per error / page / journey??
//could have every error message in config. would we want to do that?
public enum ProfessionalReferralError
{
    SingleTextboxPage_Invalid,
    //todo: could have generic TellTheService enums, and then have add the different error messages to the interface?
    // ^^ but that wouldn't work well if we want to centralise all error messages in config
    WhySupport_NothingEntered,
    WhySupport_TooLong,
    ContactMethods_NothingEntered,
    ContactMethods_TooLong,
    ContactDetails_NoContactMethodsSelected,
    Consent_NoConsentSelected,
}
