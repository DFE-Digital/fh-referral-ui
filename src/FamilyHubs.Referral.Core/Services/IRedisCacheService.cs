using FamilyHubs.Referral.Core.Models;
using FamilyHubs.ServiceDirectory.Shared.Dto;

namespace FamilyHubs.Referral.Core.Services;

public interface IRedisCacheService
{
    //Service
    ServiceDto? RetrieveService();
    void StoreService(ServiceDto serviceDto);

    //Navigation - last page name
    string RetrieveLastPageName();
    void StoreCurrentPageName(string? currPage);
    void ResetLastPageName();
    void ResetOrganisationWithService();
    void StoreConnectWizzardViewModel(string key, ConnectWizzardViewModel value);
    ConnectWizzardViewModel RetrieveConnectWizzardViewModel(string key);
    void ResetConnectWizzardViewModel(string key);
    void StoreStringValue(string key, string value);
    string RetrieveStringValue(string key);
}
