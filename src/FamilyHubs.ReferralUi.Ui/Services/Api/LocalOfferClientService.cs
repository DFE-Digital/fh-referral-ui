using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.SharedKernel;
using System.Text;
using System.Text.Json;

namespace FamilyHubs.ReferralUi.Ui.Services.Api;

public interface ILocalOfferClientService
{
    Task<PaginatedList<ServiceDto>> GetLocalOffers(LocalOfferFilter filter); 
    Task<ServiceDto> GetLocalOfferById(string id);
    Task<List<ServiceDto>> GetServicesByOrganisationId(string id);
}

public class LocalOfferClientService : ApiService, ILocalOfferClientService
{
    public LocalOfferClientService(HttpClient client)
        : base(client)
    {

    }

    public async Task<PaginatedList<ServiceDto>> GetLocalOffers(LocalOfferFilter filter) 
    {
        if (string.IsNullOrEmpty(filter.Status))
            filter.Status = "active";

        StringBuilder urlBuilder = new();

        string url = GetPositionUrl(filter.ServiceType, filter.Latitude, filter.Longtitude, filter.Proximity, filter.Status, filter.PageNumber, filter.PageSize);

        urlBuilder.Append( url );
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

        if (filter.TaxonmyIds != null)
        {
            urlBuilder.Append($"&taxonmyIds={filter.TaxonmyIds}");
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
            RequestUri = new Uri(_client.BaseAddress + urlBuilder.ToString()),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<PaginatedList<ServiceDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new PaginatedList<ServiceDto>();
    }

    private static string GetPositionUrl(string? serviceType, double? latitude, double? longtitude, double? proximity, string status, int pageNumber, int pageSize)
    {
        
        if (latitude != null && longtitude != null)
        {
            if (proximity != null)
                return $"api/services?serviceType={serviceType}&status={status}&latitude={latitude}&longtitude={longtitude}&proximity={proximity}&pageNumber={pageNumber}&pageSize={pageSize}";
            else
                return $"api/services?serviceType={serviceType}&status={status}&latitude={latitude}&longtitude={longtitude}&pageNumber={pageNumber}&pageSize={pageSize}";
        }
        else
            return $"api/services?serviceType={serviceType}&status={status}&pageNumber={pageNumber}&pageSize={pageSize}";

    }

    public void AddAgeToUrl(StringBuilder url, int? minimum_age, int? maximum_age, int? given_age)
    {
        if (minimum_age != null)
        {
            url.AppendLine($"&minimum_age={minimum_age}");
        }

        if (maximum_age != null)
        {
            url.AppendLine($"&maximum_age={maximum_age}");
        }

        if (given_age != null)
        {
            url.AppendLine($"&given_age={given_age}");
        }
    }

    public void AddTextToUrl(StringBuilder url, string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            url.AppendLine($"&text={text}");
        }
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
