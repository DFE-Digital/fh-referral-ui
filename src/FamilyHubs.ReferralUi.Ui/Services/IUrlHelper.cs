using FamilyHubs.ReferralUi.Ui.Models;

namespace FamilyHubs.ReferralUi.Ui.Services;

public interface IUrlHelper
{
    string GetPath(string baseUrl, string path = "");
    string GetPath(IUserContext userContext, string baseUrl, string path = "", string prefix = "accounts");
}
