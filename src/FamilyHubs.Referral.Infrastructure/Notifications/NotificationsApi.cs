using System.Net.Http.Json;
using FamilyHubs.Notification.Api.Contracts;
using FamilyHubs.Referral.Core.Exceptions;
using FamilyHubs.Referral.Core.Notifications;
using Microsoft.Extensions.Configuration;

namespace FamilyHubs.Referral.Infrastructure.Notifications;

#pragma warning disable S1135
#pragma warning disable S125
public class NotificationsApi : INotifications //todo: , IHealthCheckUrlGroup
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static string? _endpoint;
    internal const string HttpClientName = "notifications";

    public NotificationsApi(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task SendEmailAsync(
        string emailAddress, 
        string templateId, 
        IDictionary<string, string> tokens, 
        CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient(HttpClientName);

        var tokenDic = tokens as Dictionary<string, string>
                       ?? tokens.ToDictionary(x => x.Key, x => x.Value);

        var message = new MessageDto
        {
            RecipientEmail = emailAddress,
            TemplateId = templateId,
            TemplateTokens = tokenDic
        };

        using var response = await httpClient.PostAsJsonAsync("", message, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            //throw new NotificationsClientException(response, await response.Content.ReadAsStringAsync(cancellationToken));
            //todo:
            throw new NotImplementedException();
        }
    }

    internal static string GetEndpoint(IConfiguration configuration)
    {
        const string endpointConfigKey = "Notification:Endpoint";

        // as long as the config isn't changed, the worst that can happen is we fetch more than once
        return _endpoint ??= ConfigurationException.ThrowIfNotUrl(
            endpointConfigKey,
            configuration[endpointConfigKey],
            "The notifications URL", "https://localhost:7073/api/notify");
    }

    //public static Uri HealthUrl(IConfiguration configuration)
    //{
    //    return new Uri(new Uri(GetEndpoint(configuration)), "");
    //}
}
#pragma warning restore S125
#pragma warning restore S1135
