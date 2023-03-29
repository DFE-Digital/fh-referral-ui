using System.Text.Json;
using FamilyHubs.Referral.Core.Models;

namespace FamilyHubs.Referral.Core.ApiClients
{
    public interface IPostcodeLocationClientService
    {
        Task<PostcodesIoResponse> LookupPostcode(string postcode);
    }
    public class PostcodeLocationClientService : ApiService, IPostcodeLocationClientService
    {
        private const string PostCodeUrl = "http://api.postcodes.io";
        public PostcodeLocationClientService(HttpClient client)
            : base(client)
        {
            client.BaseAddress = new Uri(PostCodeUrl);
        }

        public async Task<PostcodesIoResponse> LookupPostcode(string postcode)
        {
            using var response = await Client.GetAsync($"/postcodes/{postcode}", HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            return await JsonSerializer.DeserializeAsync<PostcodesIoResponse>(
                await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? throw new InvalidOperationException();
        }
    }
}
