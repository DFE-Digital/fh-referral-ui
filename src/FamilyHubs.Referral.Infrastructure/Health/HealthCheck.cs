using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

namespace FamilyHubs.Referral.Infrastructure.Health;

public static class HealthCheck
{
    // extension method to add health checks
    public static IServiceCollection AddSiteHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var serviceDirectoryApiUrl = configuration.GetValue<string>("ServiceDirectoryUrl");
        var referralApiUrl = configuration.GetValue<string>("ReferralApiUrl");
        var notificationApiUrl = configuration.GetValue<string>("Notification:Endpoint");
        var idamsApiUrl = configuration.GetValue<string>("Idams:Endpoint");
        //todo: postcodes io url is hardcoded! switch to find's postcodes io client
#pragma warning disable S1075
        const string postcodesIoUrl = "http://api.postcodes.io";
#pragma warning restore S1075
        var sqlServerCacheConnectionString = configuration.GetValue<string>("SqlServerCache:Connection");

        //todo: null handling. use config exception?

        // we handle API failures as Degraded, so that App Services doesn't remove or replace the instance (all instances!) due to an API being down
        services.AddHealthChecks()
            .AddUrlGroup(new Uri(postcodesIoUrl), "PostcodesIo", HealthStatus.Degraded, new[] { "ExternalAPI" })
            .AddUrlGroup(new Uri(serviceDirectoryApiUrl!), "ServiceDirectoryAPI", HealthStatus.Degraded, new[] { "InternalAPI" })
            .AddUrlGroup(new Uri(referralApiUrl!), "ReferralAPI", HealthStatus.Degraded, new[] { "InternalAPI" })
            .AddUrlGroup(new Uri(notificationApiUrl!), "NotificationAPI", HealthStatus.Degraded, new[] { "InternalAPI" })
            .AddUrlGroup(new Uri(idamsApiUrl!), "IdamsAPI", HealthStatus.Degraded, new[] { "InternalAPI" })
            .AddSqlServer(sqlServerCacheConnectionString!, failureStatus: HealthStatus.Degraded, tags: new[] { "Database" });
        //todo: check for one login, if we can
        //todo: check feedback link?

#pragma warning disable S125
        // health check UI
        // services
        //     .AddHealthChecksUI()
        //    .AddInMemoryStorage();

        // services.AddInMemoryStorage();
        // .AddInMemoryStorage();
#pragma warning restore S125

        return services;
    }

    public static WebApplication MapSiteHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return app;
    }
}