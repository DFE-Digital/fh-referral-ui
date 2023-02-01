using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Helpers;
using static FamilyHubs.ReferralUi.Ui.Infrastructure.Configuration.TempStorageConfiguration;


namespace FamilyHubs.ReferralUi.Ui.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IRedisCache _redisCache;
    private readonly int _timespanMinites;

    public RedisCacheService(IRedisCache redisCache, IConfiguration configuration)
    {
        _redisCache = redisCache;
        _timespanMinites = configuration.GetValue<int>("SessionTimeOutMinutes");
    }

    public void ResetOrganisationWithService()
    {
        _redisCache.SetStringValue(KeyOrgWithService, String.Empty, _timespanMinites);
    }

    public string RetrieveLastPageName()
    {
        return _redisCache.GetStringValue(KeyCurrentPage) ?? string.Empty;
    }

    public void StoreCurrentPageName(string? currPage)
    {
        if (currPage != null)
            _redisCache.SetStringValue(KeyCurrentPage, currPage, _timespanMinites);
    }

    public ServiceDto? RetrieveService()
    {
        return _redisCache.GetValue<ServiceDto>(KeyService);
    }

    public void StoreService(ServiceDto serviceDto)
    {
        _redisCache.SetValue<ServiceDto>(KeyService, serviceDto, _timespanMinites);
    }

    public void ResetLastPageName()
    {
        _redisCache.SetStringValue(KeyCurrentPage, String.Empty, _timespanMinites);
    }
}
