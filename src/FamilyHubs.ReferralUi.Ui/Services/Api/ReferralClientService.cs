using FamilyHubs.ReferralUi.Ui.Infrastructure.Configuration;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.SharedKernel;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace FamilyHubs.ReferralUi.Ui.Services.Api;

public interface IReferralClientService
{
    Task<PaginatedList<ReferralDto>> GetReferralsByReferrer(string referrer, int pageNumber, int pageSize, string? searchText, bool? doNotListRejected);
    Task<string> CreateReferral(ReferralDto referralDto);
    Task<string> UpdateReferral(ReferralDto referralDto);
    Task<PaginatedList<ReferralDto>> GetReferralsByOrganisationId(string id, int pageNumber, int pageSize, string? searchText, bool? doNotListRejected);
    Task<ReferralDto?> GetReferralById(string id);
    Task<string> SetReferralStatusReferral(string referralId, string status);
}

public class ReferralClientService : ApiService, IReferralClientService
{
    public ReferralClientService(HttpClient client, IOptions<ApiOptions> options)
        : base(client)
    {
        ApiOptions settings = options.Value;
        client.BaseAddress = new Uri(settings.ReferralApiUrl);
    }

    public async Task<PaginatedList<ReferralDto>> GetReferralsByReferrer(string referrer, int pageNumber, int pageSize, string? searchText, bool? doNotListRejected)
    {
        StringBuilder urlRequest = new StringBuilder();
        urlRequest.Append($"api/referrals/{referrer}?pageNumber={pageNumber}&pageSize={pageSize}");
        if (!string.IsNullOrEmpty(searchText))
        {
            urlRequest.Append($"&searchText={searchText}");
        }
        if (doNotListRejected != null)
        {
            urlRequest.Append($"&doNotListRejected={doNotListRejected}");
        }

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + urlRequest.ToString()),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<PaginatedList<ReferralDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new PaginatedList<ReferralDto>();
    }

    public async Task<PaginatedList<ReferralDto>> GetReferralsByOrganisationId(string id, int pageNumber, int pageSize, string? searchText, bool? doNotListRejected)
    {
        StringBuilder urlRequest = new StringBuilder();
        urlRequest.Append($"api/organisationreferrals/{id}?pageNumber={pageNumber}&pageSize={pageSize}");
        if (!string.IsNullOrEmpty(searchText)) 
        {
            urlRequest.Append($"&searchText={searchText}");
        }
        if (doNotListRejected != null)
        {
            urlRequest.Append($"&doNotListRejected={doNotListRejected}");
        }
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + urlRequest.ToString()),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<PaginatedList<ReferralDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new PaginatedList<ReferralDto>();
    }

    public async Task<ReferralDto?> GetReferralById(string id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + $"api/referral/{id}"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<ReferralDto>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<string> CreateReferral(ReferralDto referralDto)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_client.BaseAddress + "api/referrals"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(referralDto), Encoding.UTF8, "application/json"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        return stringResult;
    }

    public async Task<string> UpdateReferral(ReferralDto referralDto)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(_client.BaseAddress + $"api/referrals/{referralDto.Id}"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(referralDto), Encoding.UTF8, "application/json"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        return stringResult;
    }

    public async Task<string> SetReferralStatusReferral(string referralId, string status)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_client.BaseAddress + $"api/referralStatus/{referralId}/{status}"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        return stringResult;
    }
}

