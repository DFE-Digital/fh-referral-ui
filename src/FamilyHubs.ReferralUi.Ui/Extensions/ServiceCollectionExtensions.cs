using FamilyHubs.ReferralUi.Ui.Services.Api;

namespace FamilyHubs.ReferralUi.Ui.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddClientServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("AuthServiceUrl"));
        });

        builder.Services.AddHttpClient<IApiService, ApiService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApplicationServiceApi:ServiceDirectoryUrl"));
        });

        builder.Services.AddHttpClient<IPostcodeLocationClientService, PostcodeLocationClientService>(client =>
        {
            client.BaseAddress = new Uri("http://api.postcodes.io");
        });

        builder.Services.AddHttpClient<ILocalOfferClientService, LocalOfferClientService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApplicationServiceApi:ServiceDirectoryUrl"));
        }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

        builder.Services.AddHttpClient<IOpenReferralOrganisationClientService, OpenReferralOrganisationClientService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApplicationServiceApi:ServiceDirectoryUrl"));
        }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

        builder.Services.AddHttpClient<IUICacheService, UICacheService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApplicationServiceApi:ServiceDirectoryUrl"));
        }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

        builder.Services.AddHttpClient<IReferralClientService, ReferralClientService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApplicationServiceApi:ReferralApiUrl"));
        }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

        return builder;
    }
    /*
    public static IServiceCollection AddClientServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddClient<IApiService>((c, s) => new ApiService(c));
        serviceCollection.AddClient<IPostcodeLocationClientService>((c, s) => new PostcodeLocationClientService(c));
        serviceCollection.AddClient<ILocalOfferClientService>((c, s) => new LocalOfferClientService(c));
        serviceCollection.AddClient<IOpenReferralOrganisationClientService>((c, s) => new OpenReferralOrganisationClientService(c));
        serviceCollection.AddClient<IUICacheService>((c, s) => new UICacheService(c));
        serviceCollection.AddClient<IReferralClientService>((c, s) => new ReferralClientService(c, s.GetRequiredService<IOptions<ApiOptions>>()));
        return serviceCollection;
    }

    private static IServiceCollection AddClient<T>(
        this IServiceCollection serviceCollection,
        Func<HttpClient, IServiceProvider, T> instance) where T : class
    {
        _ = serviceCollection.AddTransient(s =>
        {
            var srv = s.GetService<IOptions<ApiOptions>>();
            ArgumentNullException.ThrowIfNull(srv, nameof(srv));
            ApiOptions settings = srv.Value;
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));

            var clientBuilder = new HttpClientBuilder()
                .WithDefaultHeaders()
                //.WithApimAuthorisationHeader(settings)
                .WithLogging(s.GetService<ILoggerFactory>());

            var httpClient = clientBuilder.Build();

            if (!settings.ServiceDirectoryUrl.EndsWith("/"))
            {
                settings.ServiceDirectoryUrl += "/";
            }
            if (!settings.ReferralApiUrl.EndsWith("/"))
            {
                settings.ReferralApiUrl += "/";
            }
            httpClient.BaseAddress = new Uri(settings.ServiceDirectoryUrl);

            return instance.Invoke(httpClient, s);
        });

        return serviceCollection;
    }
    */
}
