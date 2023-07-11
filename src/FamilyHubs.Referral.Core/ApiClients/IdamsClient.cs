using System.Net.Http.Json;
using FamilyHubs.Referral.Core.Exceptions;
using Microsoft.Extensions.Configuration;

namespace FamilyHubs.Referral.Core.ApiClients;

//todo: shared client/dtos
public class AccountDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public List<AccountClaimDto> Claims { get; set; } = new List<AccountClaimDto>();
}

public class AccountClaimDto
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

#pragma warning disable S125
public class IdamsClient : IIdamsClient //todo: , IHealthCheckUrlGroup
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static string? _endpoint;
    internal const string HttpClientName = "idams";

    public IdamsClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Gets the email addresses of all users associated with the given VCS organsiation,
    /// that have either the VCS Professional or VCS Dual role.
    /// </summary>
    public async Task<IEnumerable<string>> GetVcsProfessionalsEmailsAsync(
        long organisationId,
        CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient(HttpClientName);

        using var response = await httpClient.GetAsync($"/api/account/vcsprofessionallist?organisationId={organisationId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new IdamsClientException(response, await response.Content.ReadAsStringAsync(cancellationToken));
        }

        // if no accounts found, returns an empty list
        List<AccountDto>? accounts = await response.Content.ReadFromJsonAsync<List<AccountDto>>(cancellationToken: cancellationToken);
        if (accounts == null)
        {
            throw new IdamsClientException(response, "null");
        }

        return accounts.Select(a => a.Email);
    }

    internal static string GetEndpoint(IConfiguration configuration)
    {
        const string endpointConfigKey = "Idams:Endpoint";

        // as long as the config isn't changed, the worst that can happen is we fetch more than once
        return _endpoint ??= ConfigurationException.ThrowIfNotUrl(
            endpointConfigKey,
            configuration[endpointConfigKey],
            "The IDAMS API URL", "https://localhost:7030");
    }

    //public static Uri HealthUrl(IConfiguration configuration)
    //{
    //    return new Uri(new Uri(GetEndpoint(configuration)), "");
    //}
}
#pragma warning restore S125
