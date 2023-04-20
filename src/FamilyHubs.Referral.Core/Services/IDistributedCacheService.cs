using FamilyHubs.Referral.Core.Models;

namespace FamilyHubs.Referral.Core.Services;

public interface IDistributedCacheService
{
    void StoreConnectWizzardViewModel(string key, ConnectWizzardViewModel value);
    ConnectWizzardViewModel RetrieveConnectWizzardViewModel(string key);

    //This will be used on the last page after the data is saved to the api
    void ResetConnectWizzardViewModel(string key);
}
