using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralServices;

namespace FamilyHubs.ReferralUi.Ui.Services;

public interface IRedisCacheService
{
    //Service
    public OpenReferralServiceDto? RetrieveService();
    public void StoreService(OpenReferralServiceDto serviceDto);

    //Navigation - last page name
    public string RetrieveLastPageName();
    public void StoreCurrentPageName(string? currPage);
    public void ResetLastPageName();
}
