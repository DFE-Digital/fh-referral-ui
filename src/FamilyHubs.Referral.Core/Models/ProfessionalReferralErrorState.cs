
namespace FamilyHubs.Referral.Core.Models;

public record ProfessionalReferralErrorState(
    ConnectJourneyPage ErrorPage,
    ProfessionalReferralError[] Errors,
    string[]? InvalidUserInput);