using System.Text;
using System.Text.Json;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FamilyHubs.ServiceDirectory.Shared.Models;

namespace FamilyHubs.Referral.Core.ApiClients;

public interface IOrganisationClientService
{
    Task<List<KeyValuePair<TaxonomyDto, List<TaxonomyDto>>>> GetCategories();
    Task<PaginatedList<ServiceDto>> GetLocalOffers(LocalOfferFilter filter);
    Task<ServiceDto> GetLocalOfferById(string id);
    Task<OrganisationDto?> GetOrganisationDtobyIdAsync(long id);
}

public class OrganisationClientService : ApiService, IOrganisationClientService
{
    public OrganisationClientService(HttpClient client) : base(client)
    {
    }

    public async Task<List<KeyValuePair<TaxonomyDto, List<TaxonomyDto>>>> GetCategories()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(Client.BaseAddress + "api/taxonomies?taxonomyType=ServiceCategory&pageNumber=1&pageSize=99999999"),
        };

        using var response = await Client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var retVal = await JsonSerializer.DeserializeAsync<PaginatedList<TaxonomyDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var keyValuePairs = new List<KeyValuePair<TaxonomyDto, List<TaxonomyDto>>>();

        if (retVal == null)
            return keyValuePairs;

        var topLevelCategories = retVal.Items
            .Where(x => x.ParentId == null && x.TaxonomyType == TaxonomyType.ServiceCategory)
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

    public async Task<PaginatedList<ServiceDto>> GetLocalOffers(LocalOfferFilter filter)
    {
        if (string.IsNullOrEmpty(filter.Status))
            filter.Status = "Active";

        var urlBuilder = new StringBuilder();

        var url = GetPositionUrl(filter.ServiceType, filter.Latitude, filter.Longitude, filter.Proximity, filter.Status, filter.PageNumber, filter.PageSize);

        urlBuilder.Append(url);
        AddTextToUrl(urlBuilder, filter.Text);
        AddAgeToUrl(urlBuilder, filter.MinimumAge, filter.MaximumAge, filter.GivenAge);

        if (filter.ServiceDeliveries != null)
        {
            urlBuilder.Append($"&serviceDeliveries={filter.ServiceDeliveries}");
        }

        if (filter.IsPaidFor != null)
        {
            urlBuilder.Append($"&isPaidFor={filter.IsPaidFor.Value}");
        }

        if (filter.TaxonomyIds != null)
        {
            urlBuilder.Append($"&taxonomyIds={filter.TaxonomyIds}");
        }

        if (filter.DistrictCode != null)
        {
            urlBuilder.Append($"&districtCode={filter.DistrictCode}");
        }

        if (filter.Languages != null)
        {
            urlBuilder.Append($"&languages={filter.Languages}");
        }

        if (filter.CanFamilyChooseLocation != null && filter.CanFamilyChooseLocation == true)
        {
            urlBuilder.Append($"&canFamilyChooseLocation={filter.CanFamilyChooseLocation.Value}");
        }

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(Client.BaseAddress + urlBuilder.ToString()),
        };

        using var response = await Client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<PaginatedList<ServiceDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
               ?? new PaginatedList<ServiceDto>();
    }

    private static string GetPositionUrl(string? serviceType, double? latitude, double? longitude, double? proximity, string status, int pageNumber, int pageSize)
    {
        return $"api/services?isSimple=True&serviceType={serviceType}&status={status}&pageNumber={pageNumber}&pageSize={pageSize}&isFamilyHub=false{(
                latitude != null ? $"&latitude={latitude}" : string.Empty)}{(
                longitude != null ? $"&longitude={longitude}" : string.Empty)}{(
                proximity != null ? $"&proximity={proximity}" : string.Empty)}";
    }

    public void AddAgeToUrl(StringBuilder url, int? minimumAge, int? maximumAge, int? givenAge)
    {
        if (minimumAge != null)
        {
            url.AppendLine($"&minimumAge={minimumAge}");
        }

        if (maximumAge != null)
        {
            url.AppendLine($"&maximumAge={maximumAge}");
        }

        if (givenAge != null)
        {
            url.AppendLine($"&givenAge={givenAge}");
        }
    }

    public void AddTextToUrl(StringBuilder url, string? text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            url.AppendLine($"&text={text}");
        }
    }

    public async Task<ServiceDto> GetLocalOfferById(string id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(Client.BaseAddress + $"api/services/{id}?isSimple=True"),
        };

        using var response = await Client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var retVal = await JsonSerializer.DeserializeAsync<ServiceDto>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        ArgumentNullException.ThrowIfNull(retVal);

        return retVal;
    }

    public async Task<OrganisationDto?> GetOrganisationDtobyIdAsync(long id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(Client.BaseAddress + $"api/organisations/{id}"),
        };

        using var response = await Client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<OrganisationDto>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}