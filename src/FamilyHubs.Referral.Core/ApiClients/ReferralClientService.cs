using System.Net.Http.Json;
using System.Text;
using FamilyHubs.ReferralService.Shared.Dto;

namespace FamilyHubs.Referral.Core.ApiClients;

public interface IReferralClientService
{
    Task<int> CreateReferral(ReferralDto referralDto, CancellationToken cancellationToken = default);
}

//todo: have single combined client (in referralshared)?
public class ReferralClientService : ApiService, IReferralClientService
{
    public ReferralClientService(HttpClient client) : base(client)
    {
    }

    public async Task<int> CreateReferral(ReferralDto referralDto, CancellationToken cancellationToken = default)
    {
        using var response = await Client.PostAsJsonAsync($"{Client.BaseAddress}api/referrals", referralDto, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new ReferralClientServiceException(response, await response.Content.ReadAsStringAsync(cancellationToken));
        }

        string referralIdBase10 = await response.Content.ReadAsStringAsync(cancellationToken);

        return int.Parse(referralIdBase10);
    }
}
