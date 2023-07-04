using FamilyHubs.Referral.Core.Notifications;
using FamilyHubs.SharedKernel.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;

namespace FamilyHubs.Referral.Infrastructure.Notifications.Extensions;

public static class NotificationsApiServiceCollectionExtension
{
    /// <summary>
    /// Adds the NotificationsApi service to enable sending (currently email) notifications to users
    /// </summary>
    /// <remarks>
    /// Policy notes:
    /// We don't add a circuit-breaker (but we might later).
    /// We might want to change the Handler lifetime from the default of 2 minutes, using
    /// .SetHandlerLifetime(TimeSpan.FromMinutes(3));
    /// it's a balance between keeping sockets open and latency in handling dns updates.
    /// </remarks>
    public static void AddNotificationsApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);

        var delay = Backoff.DecorrelatedJitterBackoffV2(
            medianFirstRetryDelay: TimeSpan.FromSeconds(1),
            retryCount: 2);

        services.AddHttpClient(NotificationsApi.HttpClientName, (serviceProvider, client) =>
        {
            client.BaseAddress = new Uri(NotificationsApi.GetEndpoint(configuration));

            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>()
                                      ?? throw new ArgumentException($"IHttpContextAccessor required for {nameof(AddNotificationsApiClient)}");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {httpContextAccessor.HttpContext!.GetBearerToken()}");
        })
            .AddPolicyHandler((callbackServices, request) => HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(delay, (result, timespan, retryAttempt, context) =>
                {
                    callbackServices.GetService<ILogger<NotificationsApi>>()?
                        .LogWarning("Delaying for {Timespan}, then making retry {RetryAttempt}.",
                            timespan, retryAttempt);
                }))
            .AddPolicyHandler(timeoutPolicy);

        services.AddTransient<INotifications, NotificationsApi>();
    }
}