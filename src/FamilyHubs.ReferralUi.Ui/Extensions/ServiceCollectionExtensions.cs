using FamilyHubs.ReferralUi.Ui.Services.Api;

namespace FamilyHubs.ReferralUi.Ui.Extensions;

public static class ServiceCollectionExtensions
{

    public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IAuthService, AuthService>(client =>
        {
            client.BaseAddress = new Uri(configuration?.GetValue<string>("AuthServiceUrl")!);
        });

        services.AddHttpClient<IApiService, ApiService>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("ApplicationServiceApi:ServiceDirectoryUrl")!);
        });

        services.AddHttpClient<IPostcodeLocationClientService, PostcodeLocationClientService>(client =>
        {
            const string postcodesIo = "http://api.postcodes.io";
            client.BaseAddress = new Uri(postcodesIo);
        });

        services.AddHttpClient<ILocalOfferClientService, LocalOfferClientService>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("ApplicationServiceApi:ServiceDirectoryUrl")!);
        }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

        services.AddHttpClient<IOrganisationClientService, OrganisationClientService>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("ApplicationServiceApi:ServiceDirectoryUrl")!);
        }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

        //services.AddHttpClient<IUICacheService, UICacheService>(client =>
        //{
        //    client.BaseAddress = new Uri(configuration.GetValue<string>("ApplicationServiceApi:ServiceDirectoryUrl")!);
        //}).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

        services.AddHttpClient<IReferralClientService, ReferralClientService>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("ApplicationServiceApi:ReferralApiUrl")!);
        }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();
    }
}
