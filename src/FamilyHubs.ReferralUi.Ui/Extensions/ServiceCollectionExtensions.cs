using FamilyHubs.ReferralUi.Ui.Services.Api;
using Microsoft.Extensions.Options;
using SFA.DAS.Http;

namespace FamilyHubs.ReferralUi.Ui.Extensions;

public static class ServiceCollectionExtensions
    public static WebApplicationBuilder AddClientServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("AuthServiceUrl"));
        });
    }
        builder.Services.AddHttpClient<IApiService, ApiService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApplicationServiceApi:ServiceDirectoryUrl"));
        });

        builder.Services.AddHttpClient<IPostcodeLocationClientService, PostcodeLocationClientService>(client =>
        _ = serviceCollection.AddTransient(s =>
            client.BaseAddress = new Uri("http://api.postcodes.io");
        });
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));
        builder.Services.AddHttpClient<ILocalOfferClientService, LocalOfferClientService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApplicationServiceApi:ServiceDirectoryUrl"));
        }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();
                .WithLogging(s.GetService<ILoggerFactory>());
        builder.Services.AddHttpClient<IOpenReferralOrganisationClientService, OpenReferralOrganisationClientService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApplicationServiceApi:ServiceDirectoryUrl"));
        }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();
            var httpClient = clientBuilder.Build();
        builder.Services.AddHttpClient<IUICacheService, UICacheService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApplicationServiceApi:ServiceDirectoryUrl"));
        }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

        builder.Services.AddHttpClient<IReferralClientService, ReferralClientService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApplicationServiceApi:ReferralApiUrl"));
        }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();
        });

        return builder;
    }
}
