namespace FamilyHubs.ReferralUi.Ui.Models.Configuration;

public class FooterConfiguration : IFooterConfiguration
{
    public string FamilyHubsBaseUrl { get; set; } = default!;
    public string AuthenticationAuthorityUrl { get; set; } = default!;
}
