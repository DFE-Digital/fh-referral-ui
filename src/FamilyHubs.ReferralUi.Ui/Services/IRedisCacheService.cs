using FamilyHubs.ServiceDirectory.Shared.Dto;

namespace FamilyHubs.ReferralUi.Ui.Services;

public interface IRedisCacheService
{
    //Service
    public ServiceDto? RetrieveService();
    public void StoreService(ServiceDto serviceDto);

    //Navigation - last page name
    public string RetrieveLastPageName();
    public void StoreCurrentPageName(string? currPage);
    public void ResetLastPageName();
}
