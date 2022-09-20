using System;

namespace FamilyHubs.ReferralUi.Ui.Models.Configuration;

public class HeaderConfiguration : IHeaderConfiguration
{
    public string FamilyHubsBaseUrl { get; set; } = default!;
    public string ApplicationBaseUrl { get; set; } = default!;
    public string AuthenticationAuthorityUrl { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public Uri SignOutUrl { get; set; } = default!;
    public Uri ChangeEmailReturnUrl { get; set; } = default!;
    public Uri ChangePasswordReturnUrl { get; set; } = default!;
}
