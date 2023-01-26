using System.Text.Json;
using FamilyHubs.ReferralUi.Ui.Models;

namespace FamilyHubs.ReferralUi.Ui.Services.Api
{
    public interface IPostcodeLocationClientService
    {
        Task<PostcodesIoResponse> LookupPostcode(string postcode);
    }
    public class PostcodeLocationClientService : ApiService, IPostcodeLocationClientService
    {
        public PostcodeLocationClientService(HttpClient client)
            : base(client)
        {
            client.BaseAddress = new Uri("http://api.postcodes.io");
        }

        public async Task<PostcodesIoResponse> LookupPostcode(string postcode)
        {
            using var response = await _client.GetAsync($"/postcodes/{postcode}", HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            return await JsonSerializer.DeserializeAsync<PostcodesIoResponse>(
                await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) 
                   ?? throw new InvalidOperationException();
        }
    }
}
