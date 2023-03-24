using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FamilyHubs.SharedKernel;
using System.Text;
using System.Text.Json;

namespace FamilyHubs.ReferralUi.Ui.Services.Api;

public interface IOrganisationClientService
{
    
    Task<List<KeyValuePair<TaxonomyDto, List<TaxonomyDto>>>> GetCategories();
    Task<List<OrganisationDto>> GetListOrganisations();
    Task<OrganisationWithServicesDto> GetOrganisationById(string id);
    Task<string> CreateOrganisation(OrganisationWithServicesDto organisation);
    Task<string> UpdateOrganisation(OrganisationWithServicesDto organisation);
}

public class OrganisationClientService : ApiService, IOrganisationClientService
{
    public OrganisationClientService(HttpClient client)
    : base(client)
    {

    }

    

    public async Task<List<OrganisationDto>> GetListOrganisations()
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
            RequestUri = new Uri(_client.BaseAddress + "api/taxonomies?taxonomyType=ServiceCategory&pageNumber=1&pageSize=99999999"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var retVal = await JsonSerializer.DeserializeAsync<PaginatedList<TaxonomyDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        List<KeyValuePair<TaxonomyDto, List<TaxonomyDto>>> keyValuePairs = new();

        if (retVal == null)
            return keyValuePairs;

        var topLevelCategories = retVal.Items
            .Where(x => x.ParentId == null && !x.Name.Contains("bccusergroupTestDelete") && x.TaxonomyType == TaxonomyType.ServiceCategory)
            .OrderBy(x => x.Name)
            .ToList();

        foreach (var topLevelCategory in topLevelCategories)
        {
            var subCategories = retVal.Items.Where(x => x.ParentId == topLevelCategory.Id).OrderBy(x => x.Name).ToList();
            var pair = new KeyValuePair<TaxonomyDto, List<TaxonomyDto>>(topLevelCategory, subCategories);
            keyValuePairs.Add(pair);
        }

        return keyValuePairs;
    }

    public async Task<OrganisationWithServicesDto> GetOrganisationById(string id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + $"api/organizations/{id}"),

        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();


        return await JsonSerializer.DeserializeAsync<OrganisationWithServicesDto>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new OrganisationWithServicesDto{ 
        Description = "",
        AdminAreaCode =  "",
        Name ="",
        OrganisationType = OrganisationType.NotSet
        };
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
