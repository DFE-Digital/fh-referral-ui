using FamilyHubs.ReferralUi.Ui.Extensions;
using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services;
using FamilyHubs.ServiceDirectory.Shared.Helpers;
using MassTransit;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Events;

namespace FamilyHubs.ReferralUi.Ui;

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

        services.AddTransient<IRedisCache, RedisCache>();
        services.AddTransient<IRedisCacheService, RedisCacheService>();

        // Add services to the container.
        services.AddRazorPages();

        if (!configuration.GetValue<bool>("UseRabbitMQ")) return;

        var rabbitMqSettings = configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();
        services.AddMassTransit(mt =>
            mt.UsingRabbitMq((_, cfg) =>
            {
                cfg.Host(rabbitMqSettings!.Uri, "/", c =>
                {
                    c.Username(rabbitMqSettings.UserName);
                    c.Password(rabbitMqSettings.Password);
                });
            }));
    }

    public static IServiceProvider ConfigureWebApplication(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        return app.Services;
    }
}
