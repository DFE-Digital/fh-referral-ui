using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.SharedKernel;
using System.Text.Json;

namespace FamilyHubs.ReferralUi.Ui.Services.Api;

public interface ILocalOfferClientService
{
    Task<PaginatedList<ServiceDto>> GetLocalOffers(LocalOfferFilter filter); // string serviceType, string status, int? minimum_age, int? maximum_age, int? given_age, string? districtCode, double? latitude, double? longtitude, double? proximity, int pageNumber, int pageSize, string text, string? serviceDeliveries, bool? isPaidFor, string? taxonmyIds, string? languages, bool? canFamilyChooseLocation);
    Task<ServiceDto> GetLocalOfferById(string id);
    Task<List<ServiceDto>> GetServicesByOrganisationId(string id);
}

public class LocalOfferClientService : ApiService, ILocalOfferClientService
{
    public LocalOfferClientService(HttpClient client)
        : base(client)
    {

    }

    public async Task<PaginatedList<ServiceDto>> GetLocalOffers(LocalOfferFilter filter) // string? serviceType, string status, int? minimum_age, int? maximum_age, int? given_age, string? districtCode, double? latitude, double? longtitude, double? proximity, int pageNumber, int pageSize, string text, string? serviceDeliveries, bool? isPaidFor, string? taxonmyIds, string? languages, bool? canFamilyChooseLocation)
    {
        if (string.IsNullOrEmpty(filter.Status))
            filter.Status = "active";

        string url = GetPositionUrl(filter.ServiceType, filter.Latitude, filter.Longtitude, filter.Proximity, filter.Status, filter.PageNumber, filter.PageSize);
        url = AddTextToUrl(url, filter.Text);
        url = AddAgeToUrl(url, filter.MinimumAge, filter.MaximumAge, filter.GivenAge);
        

        if (filter.ServiceDeliveries != null)
        {
            url += $"&serviceDeliveries={filter.ServiceDeliveries}";
        }

        if (filter.IsPaidFor != null)
        {
            url += $"&isPaidFor={filter.IsPaidFor.Value}";
        }

        if (filter.TaxonmyIds != null)
        {
            url += $"&taxonmyIds={filter.TaxonmyIds}";
        }

        if (filter.DistrictCode != null)
        {
            url += $"&districtCode={filter.DistrictCode}";
        }

        if (filter.Languages != null)
        {
            url += $"&languages={filter.Languages}";
        }

        if (filter.CanFamilyChooseLocation != null && filter.CanFamilyChooseLocation == true)
        {
            url += $"&canFamilyChooseLocation={filter.CanFamilyChooseLocation.Value}";
        }

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + url),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<PaginatedList<ServiceDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new PaginatedList<ServiceDto>();
    }

    private static string GetPositionUrl(string? serviceType, double? latitude, double? longtitude, double? proximity, string status, int pageNumber, int pageSize)
    {
        string url = string.Empty;
        if (latitude != null && longtitude != null)
        {
            if (proximity != null)
                url = $"api/services?serviceType={serviceType}&status={status}&latitude={latitude}&longtitude={longtitude}&proximity={proximity}&pageNumber={pageNumber}&pageSize={pageSize}";
            else
                url = $"api/services?serviceType={serviceType}&status={status}&latitude={latitude}&longtitude={longtitude}&pageNumber={pageNumber}&pageSize={pageSize}";
        }
        else
            url = $"api/services?serviceType={serviceType}&status={status}&pageNumber={pageNumber}&pageSize={pageSize}";

        return url;
    }

    public string AddAgeToUrl(string url, int? minimum_age, int? maximum_age, int? given_age)
    {
        if (minimum_age != null)
        {
            url += $"&minimum_age={minimum_age}";
        }

        if (maximum_age != null)
        {
            url += $"&maximum_age={maximum_age}";
        }

        if (given_age != null)
        {
            url += $"&given_age={given_age}";
        }

        return url;
    }

    public string AddTextToUrl(string url, string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            url += $"&text={text}";
        }

        return url;
    }

    public async Task<ServiceDto> GetLocalOfferById(string id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + $"api/services/{id}"),

        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var retVal = await JsonSerializer.DeserializeAsync<ServiceDto>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        ArgumentNullException.ThrowIfNull(retVal);

        return retVal;
    }

    public async Task<List<ServiceDto>> GetServicesByOrganisationId(string id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + $"api/organisationservices/{id}"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<ServiceDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ServiceDto>();
    }
}
