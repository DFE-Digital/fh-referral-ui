namespace FamilyHubs.Referral.Web.Models;

//todo: error enum per error / page / journey??
//could have every error message in config. would we want to do that?
public enum ProfessionalReferralError
{
    ContactDetails_NoContactMethodsSelected,
    Consent_NoConsentSelected,
}
