using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.SharedKernel;
using System.Text;
using System.Text.Json;

namespace FamilyHubs.ReferralUi.Ui.Services.Api;

public interface IOpenReferralOrganisationClientService
{
    Task<PaginatedList<TaxonomyDto>> GetTaxonomyList(int pageNumber = 1, int pageSize = 10);
    Task<List<KeyValuePair<TaxonomyDto, List<TaxonomyDto>>>> GetCategories();
    Task<List<OrganisationDto>> GetListOpenReferralOrganisations();
    Task<OrganisationWithServicesDto> GetOpenReferralOrganisationById(string id);
    Task<string> CreateOrganisation(OrganisationWithServicesDto organisation);
    Task<string> UpdateOrganisation(OrganisationWithServicesDto organisation);
}

public class OpenReferralOrganisationClientService : ApiService, IOpenReferralOrganisationClientService
{
    public OpenReferralOrganisationClientService(HttpClient client)
    : base(client)
    {

    }

    public async Task<PaginatedList<TaxonomyDto>> GetTaxonomyList(int pageNumber = 1, int pageSize = 10)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + $"api/taxonomies?pageNumber={pageNumber}&pageSize={pageSize}"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<PaginatedList<TaxonomyDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new PaginatedList<TaxonomyDto>();

    }

    public async Task<List<OrganisationDto>> GetListOpenReferralOrganisations()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + "api/organizations"),

        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<OrganisationDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrganisationDto>();

    }

    public async Task<List<KeyValuePair<TaxonomyDto, List<TaxonomyDto>>>> GetCategories()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + "api/taxonomies?pageNumber=1&pageSize=99999999"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var retVal = await JsonSerializer.DeserializeAsync<PaginatedList<TaxonomyDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        List<KeyValuePair<TaxonomyDto, List<TaxonomyDto>>> keyValuePairs = new();

        if (retVal == null)
            return keyValuePairs;

        var topLevelCategories = retVal.Items.Where(x => x.Parent == null && !x.Name.Contains("bccusergroupTestDelete")).OrderBy(x => x.Name).ToList();

        foreach (var topLevelCategory in topLevelCategories)
        {
            var subCategories = retVal.Items.Where(x => x.Parent == topLevelCategory.Id).OrderBy(x => x.Name).ToList();
            var pair = new KeyValuePair<TaxonomyDto, List<TaxonomyDto>>(topLevelCategory, subCategories);
            keyValuePairs.Add(pair);
        }

        return keyValuePairs;
    }

    public async Task<OrganisationWithServicesDto> GetOpenReferralOrganisationById(string id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + $"api/organizations/{id}"),

        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();


        return await JsonSerializer.DeserializeAsync<OrganisationWithServicesDto>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new OrganisationWithServicesDto(
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

    public async Task<string> CreateOrganisation(OrganisationWithServicesDto organisation)
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

    public async Task<string> UpdateOrganisation(OrganisationWithServicesDto organisation)
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
