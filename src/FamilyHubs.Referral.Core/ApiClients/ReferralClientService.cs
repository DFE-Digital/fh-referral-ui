using System.Net;
using FamilyHubs.ReferralService.Shared.Models;
using System.Net.Http.Json;
using FamilyHubs.ReferralService.Shared.Dto.CreateUpdate;
using FamilyHubs.ReferralService.Shared.Dto.Metrics;

namespace FamilyHubs.Referral.Core.ApiClients;

public interface IReferralClientService
{
    Task<(ReferralResponse, HttpStatusCode)> CreateReferral(CreateReferralDto createReferralDto, CancellationToken cancellationToken = default);
    Task UpdateConnectionRequestsSentMetric(UpdateConnectionRequestsSentMetricDto metric, CancellationToken cancellationToken = default);
}

//todo: have single combined client (in referralshared)?
public class ReferralClientService : ApiService, IReferralClientService
{
    public ReferralClientService(HttpClient client) : base(client)
    {
    }

    public async Task<(ReferralResponse, HttpStatusCode)> CreateReferral(CreateReferralDto createReferralDto, CancellationToken cancellationToken = default)
    {
        using var response = await Client.PostAsJsonAsync($"{Client.BaseAddress}api/referrals", createReferralDto, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new ReferralClientServiceException(response, await response.Content.ReadAsStringAsync(cancellationToken));
        }

        ReferralResponse? referralResponse = await response.Content.ReadFromJsonAsync<ReferralResponse>(cancellationToken: cancellationToken);

        if (referralResponse is null)
        {
            // the only time it'll be null, is if the API returns "null"
            // (see https://stackoverflow.com/questions/71162382/why-are-the-return-types-of-nets-system-text-json-jsonserializer-deserialize-m)
            // unlikely, but possibly (pass new MemoryStream(Encoding.UTF8.GetBytes("null")) to see it actually return null)
            // note we hard-code passing "null", rather than messing about trying to rewind the stream, as this is such a corner case and we want to let the deserializer take advantage of the async stream (in the happy case)
            throw new ReferralClientServiceException(response, "null");
        }

        return (referralResponse, response.StatusCode);
    }

    public async Task UpdateConnectionRequestsSentMetric(
        UpdateConnectionRequestsSentMetricDto metric,
        CancellationToken cancellationToken = default)
    {
        using var response = await Client.PutAsJsonAsync($"{Client.BaseAddress}api/metrics/connection-request", metric, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new ReferralClientServiceException(response, "");
        }
    }
}
