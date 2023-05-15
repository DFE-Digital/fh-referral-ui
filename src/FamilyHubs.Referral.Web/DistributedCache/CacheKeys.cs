using FamilyHubs.Referral.Core.DistributedCache;

namespace FamilyHubs.Referral.Web.DistributedCache;

//todo: move this back into infrastructure?
public class CacheKeys : ICacheKeys
{
    private readonly string _sessionId;

    public CacheKeys(IHttpContextAccessor httpContextAccessor)
    {
        _sessionId = httpContextAccessor.HttpContext!.Session.Id;
    }

    public string ConnectionRequest => SessionNamespaced("PR");

    private string SessionNamespaced(string key)
    {
        return $"{_sessionId}{key}";
    }
}