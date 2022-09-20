using System;

namespace FamilyHubs.ReferralUi.Ui.Models.Configuration;

public interface IHeaderConfiguration
{
    string FamilyHubsBaseUrl { get; set; }
    string AuthenticationAuthorityUrl { get; set; }
    Uri ChangeEmailReturnUrl { get; set; }
    Uri ChangePasswordReturnUrl { get; set; }
    Uri SignOutUrl { get; set; }
    string ClientId { get; set; }
}
