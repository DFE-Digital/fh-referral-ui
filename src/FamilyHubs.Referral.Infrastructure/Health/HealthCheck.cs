using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Azure.Core;
using Azure.Identity;

namespace FamilyHubs.Referral.Infrastructure.Health;

public static class HealthCheck
{
    public enum UrlType
    {
        InternalApi,
        ExternalApi,
        InternalSite,
        ExternalSite
    }

    public static IHealthChecksBuilder AddApi(
        this IHealthChecksBuilder builder,
        string name,
        string configKey,
        IConfiguration configuration,
        UrlType urlType = UrlType.InternalApi)
    {
        string? apiUrl = configuration.GetValue<string>(configKey);

        // Only add the health check if the config key is set.
        // Either the API is optional (or not used locally) and missing intentionally,
        // in which case there's no need to add the health check,
        // or it's required, but in that case, the real consumer of the API should
        // continue to throw it's own relevant exception
        if (!string.IsNullOrEmpty(apiUrl))
        {
            if (urlType == UrlType.InternalApi)
            {
                //todo: add "/health" endpoints to all APIs
                apiUrl += "/api/info";
            }

            // we handle API failures as Degraded, so that App Services doesn't remove or replace the instance (all instances!) due to an API being down
            builder.AddUrlGroup(new Uri(apiUrl), name, HealthStatus.Degraded,
                new[] {urlType.ToString()});
        }

        return builder;
    }

    public static IServiceCollection AddSiteHealthChecks(
        this IServiceCollection services,
        IConfiguration config)
    {
        var oneLoginUrl = config.GetValue<string>("GovUkOidcConfiguration:Oidc:BaseUrl");
        var sqlServerCacheConnectionString = config.GetValue<string>("SqlServerCache:Connection");

        var keyVaultKey = config.GetValue<string>("DataProtection:KeyIdentifier");
        int keysIndex = keyVaultKey!.IndexOf("/keys/");
        string keyVaultUrl = keyVaultKey[..keysIndex];
        string keyName = keyVaultKey[(keysIndex + 6)..];

        //todo: dataprotectionoptions is internal and clashes with a MS class
        // add extension to IHealthChecksBuilder to add health checks for keyvault (and sql) for dataprotection. single call to add DataProtection health checks, with overridable tag defaulting to DataProtection
        // add extension to common clients etc. that way centralise where config comes from and config exceptions

        //todo: null handling. use config exception?

        TokenCredential keyVaultCredentials = new ClientSecretCredential(
            config.GetValue<string>("DataProtection:TenantId"),
            config.GetValue<string>("DataProtection:ClientId"),
            config.GetValue<string>("DataProtection:ClientSecret"));

        // we handle API failures as Degraded, so that App Services doesn't remove or replace the instance (all instances!) due to an API being down
        var healthCheckBuilder = services.AddHealthChecks()
            .AddIdentityServer(new Uri(oneLoginUrl!), name: "One Login", failureStatus: HealthStatus.Degraded, tags: new[] { "ExternalAPI" })
#pragma warning disable S1075
            //todo: postcodes io url is hardcoded! switch to find's postcodes io client? strategic switch coming
            .AddUrlGroup(new Uri("http://api.postcodes.io"), "PostcodesIo", HealthStatus.Degraded, new[] { "ExternalAPI" })
#pragma warning restore S1075
            //todo: family hubs helper?
            .AddApi("Feedback Site", "FamilyHubsUi:FeedbackUrl", config)
            .AddApi("Service Directory API", "ServiceDirectoryUrl", config)
            .AddApi("Referral API", "ReferralApiUrl", config)
            .AddApi("Notification API", "Notification:Endpoint", config)
            .AddApi("Idams API", "Idams:Endpoint", config)
            .AddSqlServer(sqlServerCacheConnectionString!, failureStatus: HealthStatus.Degraded, tags: new[] { "Database" })
            //todo: tag as AKV, name as Data Protection Key?
            .AddAzureKeyVault(new Uri(keyVaultUrl), keyVaultCredentials, s => s.AddKey(keyName), name:"Azure Key Vault", failureStatus: HealthStatus.Degraded, tags: new[] { "Infrastructure" });

        // not usually set running locally
        string? aiInstrumentationKey = config.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");
        if (!string.IsNullOrEmpty(aiInstrumentationKey))
        {
            //todo: check in dev env
            healthCheckBuilder.AddAzureApplicationInsights(aiInstrumentationKey!, "App Insights", HealthStatus.Degraded, new[] {"Infrastructure"});
        }

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