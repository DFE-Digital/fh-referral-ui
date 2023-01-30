using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralServices;
using FamilyHubs.SharedKernel;
using System.Text.Json;

namespace FamilyHubs.ReferralUi.Ui.Services.Api;

public interface ILocalOfferClientService
{
    Task<PaginatedList<OpenReferralServiceDto>> GetLocalOffers(string serviceType, string status, int? minimum_age, int? maximum_age, int? given_age, string? districtCode, double? latitude, double? longtitude, double? proximity, int pageNumber, int pageSize, string text, string? serviceDeliveries, bool? isPaidFor, string? taxonmyIds, string? languages, bool? canFamilyChooseLocation);
    Task<OpenReferralServiceDto> GetLocalOfferById(string id);
    //Task<PaginatedList<TestItem>> GetTestCommand(double latitude, double logtitude, double meters);

    Task<List<OpenReferralServiceDto>> GetServicesByOrganisationId(string id);
}

public class LocalOfferClientService : ApiService, ILocalOfferClientService
{
    public LocalOfferClientService(HttpClient client)
        : base(client)
    {

    }

    public async Task<PaginatedList<OpenReferralServiceDto>> GetLocalOffers(string? serviceType, string status, int? minimum_age, int? maximum_age, int? given_age, string? districtCode, double? latitude, double? longtitude, double? proximity, int pageNumber, int pageSize, string text, string? serviceDeliveries, bool? isPaidFor, string? taxonmyIds, string? languages, bool? canFamilyChooseLocation)
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

        return await JsonSerializer.DeserializeAsync<PaginatedList<OpenReferralServiceDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new PaginatedList<OpenReferralServiceDto>();
    }

    public async Task<OpenReferralServiceDto> GetLocalOfferById(string id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + $"api/services/{id}"),

        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var retVal = await JsonSerializer.DeserializeAsync<OpenReferralServiceDto>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        ArgumentNullException.ThrowIfNull(retVal, nameof(retVal));

        return retVal;
    }

    public async Task<List<OpenReferralServiceDto>> GetServicesByOrganisationId(string id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + $"api/organisationservices/{id}"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<OpenReferralServiceDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OpenReferralServiceDto>();
    }
}
