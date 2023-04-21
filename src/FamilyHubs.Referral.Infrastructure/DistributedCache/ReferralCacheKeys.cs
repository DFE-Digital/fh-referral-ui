using FamilyHubs.Referral.Core.DistributedCache;
using Microsoft.AspNetCore.Http;

namespace FamilyHubs.Referral.Infrastructure.DistributedCache;

public class ReferralCacheKeys : IReferralCacheKeys
{
    private readonly string _sessionId;

    public ReferralCacheKeys(IHttpContextAccessor httpContextAccessor)
    {
        _sessionId = httpContextAccessor.HttpContext!.Session.Id;
    }

    public string ProfessionalReferral => SessionNamespaced("PR");

    private string SessionNamespaced(string key)
    {
        return $"{_sessionId}{key}";
    }
}