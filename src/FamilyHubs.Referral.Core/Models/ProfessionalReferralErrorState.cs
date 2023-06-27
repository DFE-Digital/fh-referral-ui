
namespace FamilyHubs.Referral.Core.Models;

public record ProfessionalReferralErrorState(
    ConnectJourneyPage ErrorPage,
    ErrorId[] Errors,
    string[]? InvalidUserInput);