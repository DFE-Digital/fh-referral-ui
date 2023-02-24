using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ServiceDirectory.Shared.Dto;

namespace FamilyHubs.ReferralUi.Ui.Services;

public interface IRedisCacheService
{
    //Service
    ServiceDto? RetrieveService();
    void StoreService(ServiceDto serviceDto);

    //Navigation - last page name
    string RetrieveLastPageName();
    void StoreCurrentPageName(string? currPage);
    void ResetLastPageName();
    string GetUserKey();
    void ResetOrganisationWithService();
    internal void StoreConnectWizzardViewModel(string key, ConnectWizzardViewModel value);
    internal ConnectWizzardViewModel RetrieveConnectWizzardViewModel(string key);
    internal void ResetConnectWizzardViewModel(string key);
}
