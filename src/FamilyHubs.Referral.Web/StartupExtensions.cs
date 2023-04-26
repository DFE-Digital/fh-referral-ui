using FamilyHubs.Referral.Core.ApiClients;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;

namespace FamilyHubs.Referral.Web;

public static class StartupExtensions
{
    public static void ConfigureHost(this WebApplicationBuilder builder)
    {
        // ApplicationInsights
        builder.Host.UseSerilog((_, services, loggerConfiguration) =>
        {
            var logLevelString = builder.Configuration["LogLevel"];

            var parsed = Enum.TryParse<LogEventLevel>(logLevelString, out var logLevel);

            loggerConfiguration.WriteTo.ApplicationInsights(
                services.GetRequiredService<TelemetryConfiguration>(),
                TelemetryConverter.Traces,
                parsed ? logLevel : LogEventLevel.Warning);

            loggerConfiguration.WriteTo.Console(
                parsed ? logLevel : LogEventLevel.Warning);
        });
    }

    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationInsightsTelemetry();

        // Add services to the container.
        services.AddHttpClients(configuration);

        services.AddWebUiServices(configuration);

        // Add services to the container.
        services.AddRazorPages();

        services.AddFamilyHubs(configuration);
    }

    public static void AddWebUiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        var sessionTimeOutMinutes = configuration.GetValue<int>("SessionTimeOutMinutes");
        services.AddSession(options => {
            options.IdleTimeout = TimeSpan.FromMinutes(sessionTimeOutMinutes);
        });

        // the expiration should be longer than the session timeout,
        // so that the cache entry is not removed before the session expires.
        // (we make the session quite a bit longer, in case the user is keeping the session alive,
        // without updating the redis cache, e.g. by refreshing the safeguarding page.)
        services.AddReferralDistributedCache(
            configuration["RedisCache:Connection"],
            int.Parse(configuration["RedisCache:SlidingExpirationInMinutes"] ?? "240"));
    }

    public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IPostcodeLocationClientService, PostcodeLocationClientService>(client =>
        {
            const string PostcodesIo = "http://api.postcodes.io";
            client.BaseAddress = new Uri(PostcodesIo);
        });

        services.AddHttpClient<IOrganisationClientService, OrganisationClientService>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("ServiceDirectoryUrl")!);
        });
    }
    public static IServiceProvider ConfigureWebApplication(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.UseFamilyHubs();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

#if use_https
        app.UseHttpsRedirection();
#endif

        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession();

        //app.UseAuthentication();
        //app.UseAuthorization();

        app.MapRazorPages();

        return app.Services;
    }
}
