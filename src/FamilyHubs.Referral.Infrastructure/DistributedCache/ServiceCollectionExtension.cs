using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Infrastructure.DistributedCache;
using Microsoft.Extensions.Caching.Distributed;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    // passing Action<ReferralDistributedCacheOptions> would follow the standard pattern, but as this is not shared, keep it simple
    public static IServiceCollection AddReferralDistributedCache(
        this IServiceCollection services,
        string? connectionString,
        int slidingExpirationInMinutes)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = "ReferralWeb";
        });

        services.AddTransient<IConnectionRequestDistributedCache, ConnectionRequestDistributedCache>();

        // there's currently only one, so this should be fine
        services.AddSingleton(new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(slidingExpirationInMinutes)
        });

        return services;
    }
}
