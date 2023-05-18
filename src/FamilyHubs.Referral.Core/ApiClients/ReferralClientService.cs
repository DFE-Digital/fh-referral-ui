using FamilyHubs.ServiceDirectory.Shared.Dto;
using System.Text;

namespace FamilyHubs.Referral.Core.ApiClients;


public interface IReferralClientService
{
    Task<string> CreateReferral(ReferralDto referralDto);
}
public class ReferralClientService : ApiService, IReferralClientService
{
    public ReferralClientService(HttpClient client) : base(client)
    {
    }

    public async Task<string> CreateReferral(ReferralDto referralDto)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(Client.BaseAddress + "api/referrals"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(referralDto), Encoding.UTF8, "application/json"),
        };

        using var response = await Client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        return stringResult;
    }
}
