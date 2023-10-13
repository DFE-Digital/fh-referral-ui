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
    public enum ApiType
    {
        Internal,
        External
    }

    public static IHealthChecksBuilder AddApi(
        this IHealthChecksBuilder builder,
        string name,
        string configKey,
        IConfiguration configuration,
        ApiType apiType = ApiType.Internal)
    {
        string? apiUrl = configuration.GetValue<string>(configKey);

        // Only add the health check if the config key is set.
        // Either the API is optional (or not used locally) and missing intentionally,
        // in which case there's no need to add the health check,
        // or it's required, but in that case, the real consumer of the API should
        // continue to throw it's own relevant exception
        if (!string.IsNullOrEmpty(apiUrl))
        {
            // we handle API failures as Degraded, so that App Services doesn't remove or replace the instance (all instances!) due to an API being down
            builder.AddUrlGroup(new Uri(apiUrl), name, HealthStatus.Degraded,
                new[] {apiType == ApiType.External ? "ExternalAPI" : "InternalAPI"});
        }

        return builder;
    }

    public static IServiceCollection AddSiteHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var referralApiUrl = configuration.GetValue<string>("ReferralApiUrl");
        var notificationApiUrl = configuration.GetValue<string>("Notification:Endpoint");
        var idamsApiUrl = configuration.GetValue<string>("Idams:Endpoint");
        //todo: postcodes io url is hardcoded! switch to find's postcodes io client? strategic switch coming
#pragma warning disable S1075
        const string postcodesIoUrl = "http://api.postcodes.io";
#pragma warning restore S1075
        var oneLoginUrl = configuration.GetValue<string>("GovUkOidcConfiguration:Oidc:BaseUrl");
        var sqlServerCacheConnectionString = configuration.GetValue<string>("SqlServerCache:Connection");
        string? feedbackUrl = configuration.GetValue<string>("FamilyHubsUi:FeedbackUrl");

        var keyVaultKey = configuration.GetValue<string>("DataProtection:KeyIdentifier");
        int keysIndex = keyVaultKey!.IndexOf("/keys/");
        string keyVaultUrl = keyVaultKey[..keysIndex];
        string keyName = keyVaultKey[(keysIndex + 6)..];

        //todo: dataprotectionoptions is internal and clashes with a MS class
        // add extension to IHealthChecksBuilder to add health checks for keyvault (and sql) for dataprotection. single call to add DataProtection health checks, with overridable tag defaulting to DataProtection
        // add extension to common clients etc. that way centralise where config comes from and config exceptions

        //todo: null handling. use config exception?

        TokenCredential keyVaultCredentials = new ClientSecretCredential(
            configuration.GetValue<string>("DataProtection:TenantId"),
            configuration.GetValue<string>("DataProtection:ClientId"),
            configuration.GetValue<string>("DataProtection:ClientSecret"));

        // we handle API failures as Degraded, so that App Services doesn't remove or replace the instance (all instances!) due to an API being down
        var healthCheckBuilder = services.AddHealthChecks()
            .AddIdentityServer(new Uri(oneLoginUrl!), name: "One Login", failureStatus: HealthStatus.Degraded, tags: new[] { "ExternalAPI" })
            .AddUrlGroup(new Uri(postcodesIoUrl), "PostcodesIo", HealthStatus.Degraded, new[] { "ExternalAPI" })
            .AddUrlGroup(new Uri(feedbackUrl!), "Feedback", HealthStatus.Degraded, new[] { "ExternalSite" })
            .AddApi("Service Directory API", "ServiceDirectoryUrl", configuration)
            .AddUrlGroup(new Uri(referralApiUrl!), "ReferralAPI", HealthStatus.Degraded, new[] { "InternalAPI" })
            .AddUrlGroup(new Uri(notificationApiUrl!), "NotificationAPI", HealthStatus.Degraded, new[] { "InternalAPI" })
            .AddUrlGroup(new Uri(idamsApiUrl!), "IdamsAPI", HealthStatus.Degraded, new[] { "InternalAPI" })
            .AddSqlServer(sqlServerCacheConnectionString!, failureStatus: HealthStatus.Degraded, tags: new[] { "Database" })
            //todo: tag as AKV, name as Data Protection Key?
            .AddAzureKeyVault(new Uri(keyVaultUrl!), keyVaultCredentials, s => s.AddKey(keyName), name:"Azure Key Vault", failureStatus: HealthStatus.Degraded, tags: new[] { "Infrastructure" });

        // not usually set running locally
        string? aiInstrumentationKey = configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");
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