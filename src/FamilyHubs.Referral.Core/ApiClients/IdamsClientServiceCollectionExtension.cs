using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using FamilyHubs.SharedKernel.Identity;

namespace FamilyHubs.Referral.Core.ApiClients;

public static class IdamsClientServiceCollectionExtension
{
    /// <summary>
    /// Adds the IdamsClient service to call Idams endpoints
    /// </summary>
    /// <remarks>
    /// Policy notes:
    /// We don't add a circuit-breaker (but we might later).
    /// We might want to change the Handler lifetime from the default of 2 minutes, using
    /// .SetHandlerLifetime(TimeSpan.FromMinutes(3));
    /// it's a balance between keeping sockets open and latency in handling dns updates.
    /// </remarks>
    public static void AddIdamsClient(this IServiceCollection services, IConfiguration configuration)
    {
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);

        var delay = Backoff.DecorrelatedJitterBackoffV2(
            medianFirstRetryDelay: TimeSpan.FromSeconds(1),
            retryCount: 2);

        services.AddHttpClient(IdamsClient.HttpClientName, (serviceProvider, client) =>
        {
            client.BaseAddress = new Uri(IdamsClient.GetEndpoint(configuration));

            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>()
                                      ?? throw new ArgumentException($"IHttpContextAccessor required for {nameof(AddIdamsClient)}");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {httpContextAccessor.HttpContext!.GetBearerToken()}");
        })
            .AddPolicyHandler((callbackServices, request) => HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(delay, (result, timespan, retryAttempt, context) =>
                {
                    callbackServices.GetService<ILogger<IdamsClient>>()?
                        .LogWarning("Delaying for {Timespan}, then making retry {RetryAttempt}.",
                            timespan, retryAttempt);
                }))
            .AddPolicyHandler(timeoutPolicy);

        services.AddTransient<IIdamsClient, IdamsClient>();
    }
}