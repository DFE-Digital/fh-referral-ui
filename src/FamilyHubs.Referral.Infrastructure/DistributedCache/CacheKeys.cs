using FamilyHubs.Referral.Core.DistributedCache;
using Microsoft.AspNetCore.Http;

namespace FamilyHubs.Referral.Infrastructure.DistributedCache;

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