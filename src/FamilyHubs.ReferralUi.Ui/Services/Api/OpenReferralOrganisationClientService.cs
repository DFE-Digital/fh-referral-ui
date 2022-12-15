using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralTaxonomys;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OrganisationType;
using FamilyHubs.SharedKernel;
using System.Text;
using System.Text.Json;

namespace FamilyHubs.ReferralUi.Ui.Services.Api;

public interface IOpenReferralOrganisationClientService
{
    Task<PaginatedList<OpenReferralTaxonomyDto>> GetTaxonomyList(int pageNumber = 1, int pageSize = 10);
    Task<List<KeyValuePair<OpenReferralTaxonomyDto, List<OpenReferralTaxonomyDto>>>> GetCategories();
    Task<List<OpenReferralOrganisationDto>> GetListOpenReferralOrganisations();
    Task<OpenReferralOrganisationWithServicesDto> GetOpenReferralOrganisationById(string id);
    Task<string> CreateOrganisation(OpenReferralOrganisationWithServicesDto organisation);
    Task<string> UpdateOrganisation(OpenReferralOrganisationWithServicesDto organisation);
}

public class OpenReferralOrganisationClientService : ApiService, IOpenReferralOrganisationClientService
{
    public OpenReferralOrganisationClientService(HttpClient client)
    : base(client)
    {

    }

    public async Task<PaginatedList<OpenReferralTaxonomyDto>> GetTaxonomyList(int pageNumber = 1, int pageSize = 10)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + $"api/taxonomies?pageNumber={pageNumber}&pageSize={pageSize}"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<PaginatedList<OpenReferralTaxonomyDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new PaginatedList<OpenReferralTaxonomyDto>();

    }

    public async Task<List<OpenReferralOrganisationDto>> GetListOpenReferralOrganisations()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + "api/organizations"),

        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<OpenReferralOrganisationDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OpenReferralOrganisationDto>();

    }

    public async Task<List<KeyValuePair<OpenReferralTaxonomyDto, List<OpenReferralTaxonomyDto>>>> GetCategories()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + "api/taxonomies?pageNumber=1&pageSize=99999999"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var retVal = await JsonSerializer.DeserializeAsync<PaginatedList<OpenReferralTaxonomyDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        List<KeyValuePair<OpenReferralTaxonomyDto, List<OpenReferralTaxonomyDto>>> keyValuePairs = new();

        if (retVal == null)
            return keyValuePairs;

        var topLevelCategories = retVal.Items.Where(x => x.Parent == null && !x.Name.Contains("bccusergroupTestDelete")).OrderBy(x => x.Name).ToList();

        foreach (var topLevelCategory in topLevelCategories)
        {
            var subCategories = retVal.Items.Where(x => x.Parent == topLevelCategory.Id).OrderBy(x => x.Name).ToList();
            var pair = new KeyValuePair<OpenReferralTaxonomyDto, List<OpenReferralTaxonomyDto>>(topLevelCategory, subCategories);
            keyValuePairs.Add(pair);
        }

        return keyValuePairs;
    }

    public async Task<OpenReferralOrganisationWithServicesDto> GetOpenReferralOrganisationById(string id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + $"api/organizations/{id}"),

        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();


        return await JsonSerializer.DeserializeAsync<OpenReferralOrganisationWithServicesDto>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new OpenReferralOrganisationWithServicesDto(
            Guid.NewGuid().ToString()
            , new(string.Empty, string.Empty, string.Empty)
            , ""
            , null
            , null
            , null
            , null
            , null
            );
    }

    public async Task<string> CreateOrganisation(OpenReferralOrganisationWithServicesDto organisation)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_client.BaseAddress + "api/organizations"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(organisation), Encoding.UTF8, "application/json"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        return stringResult;
    }

    public async Task<string> UpdateOrganisation(OpenReferralOrganisationWithServicesDto organisation)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(_client.BaseAddress + $"api/organizations/{organisation.Id}"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(organisation), Encoding.UTF8, "application/json"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var stringResult = await response.Content.ReadAsStringAsync();
        return stringResult;
    }
}
