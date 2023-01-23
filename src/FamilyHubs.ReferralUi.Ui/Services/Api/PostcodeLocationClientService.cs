using FamilyHubs.ReferralUi.Ui.Models;
using Newtonsoft.Json;

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

            var content = await response.Content.ReadAsStringAsync();

#pragma warning disable CS8603 // Possible null reference return.
            return JsonConvert.DeserializeObject<PostcodesIoResponse>(content);
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
