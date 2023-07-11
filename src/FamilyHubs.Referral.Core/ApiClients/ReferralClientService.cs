using System.Net.Http.Json;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.SharedKernel.Security;
using System.Net.Http.Json;

namespace FamilyHubs.Referral.Core.ApiClients;

public interface IReferralClientService
{
    Task<int> CreateReferral(ReferralDto referralDto, CancellationToken cancellationToken = default);
}

//todo: have single combined client (in referralshared)?
public class ReferralClientService : ApiService, IReferralClientService
{
    private readonly ICrypto _crypto;

    public ReferralClientService(HttpClient client, ICrypto crypto) : base(client)
    {
        _crypto = crypto;
    }

    public async Task<int> CreateReferral(ReferralDto referralDto, CancellationToken cancellationToken = default)
    {
        referralDto.ReasonForSupport = _crypto.EncryptData(referralDto.ReasonForSupport);
        referralDto.EngageWithFamily = _crypto.EncryptData(referralDto.EngageWithFamily);

        using var response = await Client.PostAsJsonAsync($"{Client.BaseAddress}api/referrals", referralDto, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new ReferralClientServiceException(response, await response.Content.ReadAsStringAsync(cancellationToken));
        }

        string referralIdBase10 = await response.Content.ReadAsStringAsync(cancellationToken);

        return int.Parse(referralIdBase10);
    }
}
