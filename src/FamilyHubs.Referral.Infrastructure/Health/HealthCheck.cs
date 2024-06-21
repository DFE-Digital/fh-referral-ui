using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Azure.Core;
using Azure.Identity;
using FamilyHubs.SharedKernel.Razor.Health;

namespace FamilyHubs.Referral.Infrastructure.Health;

public static class HealthCheck
{
    public static IServiceCollection AddSiteHealthChecks(
        this IServiceCollection services,
        IConfiguration config)
    {
        var keyVaultKey = config.GetValue<string>("DataProtection:KeyIdentifier");
        int keysIndex = keyVaultKey!.IndexOf("/keys/");
        string keyVaultUrl = keyVaultKey[..keysIndex];
        string keyName = keyVaultKey[(keysIndex + 6)..];

        //todo: dataprotectionoptions is internal and clashes with a MS class
        // add extension to IHealthChecksBuilder to add health checks for keyvault (and sql) for dataprotection. single call to add DataProtection health checks, with overridable tag defaulting to DataProtection
        // add extension to common clients etc. that way centralise where config comes from and config exceptions

        //todo: do we want to health check each instance of e.g. sql server connection, in case are different?

        TokenCredential keyVaultCredentials = new ClientSecretCredential(
            config.GetValue<string>("DataProtection:TenantId"),
            config.GetValue<string>("DataProtection:ClientId"),
            config.GetValue<string>("DataProtection:ClientSecret"));

        var oneLoginUrl = config.GetValue<string>("GovUkOidcConfiguration:Oidc:BaseUrl");

        // we handle API failures as Degraded, so that App Services doesn't remove or replace the instance (all instances!) due to an API being down
        var healthCheckBuilder = services.AddFamilyHubsHealthChecks(config)
            .AddIdentityServer(new Uri(oneLoginUrl!), name: "One Login", failureStatus: HealthStatus.Degraded, tags: new[] { "ExternalAPI" })
            //todo: tag as AKV, name as Data Protection Key?
            .AddAzureKeyVault(new Uri(keyVaultUrl), keyVaultCredentials, s => s.AddKey(keyName), name:"Azure Key Vault", failureStatus: HealthStatus.Degraded, tags: new[] { "Infrastructure" });

        //todo: add helper to notification client
        var notificationApiUrl = config.GetValue<string>("Notification:Endpoint");
        if (!string.IsNullOrEmpty(notificationApiUrl))
        {
            // special case as Url contains path
            //todo: change notifications client to use host and append path
            notificationApiUrl = notificationApiUrl.Replace("/api/notify", "/api/info");
            healthCheckBuilder.AddUrlGroup(new Uri(notificationApiUrl), "Notification API", HealthStatus.Degraded,
                new[] { FhHealthChecksBuilder.UrlType.InternalApi.ToString() });
        }

        return services;
    }
}