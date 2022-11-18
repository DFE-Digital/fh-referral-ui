using FamilyHubs.ServiceDirectory.Shared.Helpers;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralServices;
using FamilyHubs.ReferralUi.Ui.Models;
using static FamilyHubs.ReferralUi.Ui.Infrastructure.Configuration.TempStorageConfiguration;


namespace FamilyHubs.ReferralUi.Ui.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IRedisCache _redisCache;

    public RedisCacheService(IRedisCache redisCache)
    {
        _redisCache = redisCache;
    }

    public void ResetOrganisationWithService()
    {
        _redisCache.SetStringValue(KeyOrgWithService, String.Empty);
    }

    public string RetrieveLastPageName()
    {
        return _redisCache.GetStringValue(KeyCurrentPage) ?? string.Empty;
    }

    public void StoreCurrentPageName(string? currPage)
    {
        if (currPage != null)
            _redisCache.SetStringValue(KeyCurrentPage, currPage);
    }

    public OpenReferralServiceDto? RetrieveService()
    {
        return _redisCache.GetValue<OpenReferralServiceDto>(KeyService);
    }

    public void StoreService(OpenReferralServiceDto serviceDto)
    {
        _redisCache.SetValue<OpenReferralServiceDto>(KeyService, serviceDto);
    }

    public void ResetLastPageName()
    {
        _redisCache.SetStringValue(KeyCurrentPage, String.Empty);
    }
}
