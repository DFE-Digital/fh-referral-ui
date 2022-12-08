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
}
