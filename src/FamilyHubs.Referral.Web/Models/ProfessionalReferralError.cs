namespace FamilyHubs.Referral.Web.Models;

//todo: error enum per error / page / journey??
//could have every error message in config. would we want to do that?
public enum ProfessionalReferralError
{
    SingleTextboxPage_Invalid,
    //todo: have generic TellTheService errors, and then have generic ones as part of the interface?
    WhySupport_NothingEntered,
    WhySupport_TooLong,
    ContactDetails_NoContactMethodsSelected,
    Consent_NoConsentSelected,
}
