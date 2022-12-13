using FamilyHubs.ReferralUi.Ui.Infrastructure.Configuration;
using FamilyHubs.ReferralUi.Ui.Services;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.ReferralUi.Ui.Extensions;

public static class ConfigureServices
{
    public static void AddWebUiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiOptions>(configuration.GetSection(ApiOptions.ApplicationServiceApi));
       
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        services.AddHttpContextAccessor();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
    }
}
