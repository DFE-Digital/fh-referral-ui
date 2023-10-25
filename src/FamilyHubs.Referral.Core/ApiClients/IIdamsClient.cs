
namespace FamilyHubs.Referral.Core.ApiClients;

public interface IIdamsClient
{
    Task<IEnumerable<string>> GetVcsProfessionalsEmailsAsync(
        long organisationId,
        CancellationToken cancellationToken = default);

    Task UpdateAccountSelfService(
        UpdateAccountSelfServiceDto accountSelfServiceDto,
        CancellationToken cancellationToken = default);
}