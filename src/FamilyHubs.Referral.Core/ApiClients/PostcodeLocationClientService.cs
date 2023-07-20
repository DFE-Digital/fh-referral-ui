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
#pragma warning disable S1075 // URIs should not be hardcoded
        private const string PostCodeUrl = "http://api.postcodes.io";
#pragma warning restore S1075 // URIs should not be hardcoded
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
