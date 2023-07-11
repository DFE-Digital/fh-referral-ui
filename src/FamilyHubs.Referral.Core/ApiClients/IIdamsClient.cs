
namespace FamilyHubs.Referral.Core.ApiClients;

public interface IIdamsClient
{
    Task<IEnumerable<string>> GetVcsProfessionalsEmailsAsync(
        long organisationId,
        CancellationToken cancellationToken = default);
}