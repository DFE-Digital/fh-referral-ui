
namespace FamilyHubs.Referral.Core;

public interface IReferralNotificationService
{
    Task OnCreateReferral(
        string laProfessionalEmailAddress,
        long serviceOrgansiationId,
        string serviceName,
        int requestNumber);
};
