namespace FamilyHubs.ReferralUi.Ui.Infrastructure.Configuration;

public class ExternalLinksConfiguration
{
    public const string FamilyHubsExternalLinksConfiguration = "ExternalLinks";

    public virtual string ManageFamilyHubsSiteUrl { get; set; } = default!;
    public virtual string CommitmentsSiteUrl { get; set; } = default!;
    
}
