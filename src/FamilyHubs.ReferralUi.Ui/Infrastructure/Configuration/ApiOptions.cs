using SFA.DAS.Http.Configuration;

namespace FamilyHubs.ReferralUi.Ui.Infrastructure.Configuration;

public class ApiOptions //: IApimClientConfiguration
{
    public const string ApplicationServiceApi = "ApplicationServiceApi";
    public string ServiceDirectoryUrl { get; set; } = "https://localhost:7022/";
    public string ReferralApiUrl { get; set; } = "https://localhost:7282";
    public string SubscriptionKey { get; set; } = default!;
    public string ApiVersion { get; set; } = default!;
}
