using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.SharedKernel;
using System.Text.Json;

namespace FamilyHubs.ReferralUi.Ui.Services.Api;

public interface ILocalOfferClientService
{
    Task<PaginatedList<ServiceDto>> GetLocalOffers(string serviceType, string status, int? minimum_age, int? maximum_age, int? given_age, string? districtCode, double? latitude, double? longtitude, double? proximity, int pageNumber, int pageSize, string text, string? serviceDeliveries, bool? isPaidFor, string? taxonmyIds, string? languages, bool? canFamilyChooseLocation);
    Task<ServiceDto> GetLocalOfferById(string id);
    //Task<PaginatedList<TestItem>> GetTestCommand(double latitude, double logtitude, double meters);

    Task<List<ServiceDto>> GetServicesByOrganisationId(string id);
}

public class LocalOfferClientService : ApiService, ILocalOfferClientService
{
    public LocalOfferClientService(HttpClient client)
        : base(client)
    {

    }

    public async Task<PaginatedList<ServiceDto>> GetLocalOffers(string? serviceType, string status, int? minimum_age, int? maximum_age, int? given_age, string? districtCode, double? latitude, double? longtitude, double? proximity, int pageNumber, int pageSize, string text, string? serviceDeliveries, bool? isPaidFor, string? taxonmyIds, string? languages, bool? canFamilyChooseLocation)
    {
        if (string.IsNullOrEmpty(status))
            status = "active";

        string url = string.Empty;
        if (latitude != null && longtitude != null)
        {
            if (proximity != null)
                url = $"api/services?serviceType={serviceType}&status={status}&latitude={latitude}&longtitude={longtitude}&proximity={proximity}&pageNumber={pageNumber}&pageSize={pageSize}&text={text}";
            else
                url = $"api/services?serviceType={serviceType}&status={status}&latitude={latitude}&longtitude={longtitude}&pageNumber={pageNumber}&pageSize={pageSize}&text={text}";
        }
            
        else
            url = $"api/services?serviceType={serviceType}&status={status}&pageNumber={pageNumber}&pageSize={pageSize}&text={text}";

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

        if (serviceDeliveries != null)
        {
            url += $"&serviceDeliveries={serviceDeliveries}";
        }

        if (isPaidFor != null)
        {
            url += $"&isPaidFor={isPaidFor.Value}";
        }

        if (taxonmyIds != null)
        {
            url += $"&taxonmyIds={taxonmyIds}";
        }

        if (districtCode != null)
        {
            url += $"&districtCode={districtCode}";
        }

        if (languages != null)
        {
            url += $"&languages={languages}";
        }

        if (canFamilyChooseLocation != null && canFamilyChooseLocation == true)
        {
            url += $"&canFamilyChooseLocation={canFamilyChooseLocation.Value}";
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
