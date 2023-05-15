using FamilyHubs.Referral.Infrastructure.DistributedCache;
using FamilyHubs.Referral.Web.DistributedCache;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Infrastructure.DistributedCache;

public class ReferralCacheKeysTests
{
    [Fact]
    public void ProfessionalReferral_WhenCalled_ReturnsSessionNamespacedKey()
    {
        var sessionId = "my_session_id";
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var mockSession = new Mock<ISession>();
        mockSession.Setup(s => s.Id).Returns(sessionId);
        mockHttpContextAccessor
            .Setup(h => h.HttpContext!.Session)
            .Returns(mockSession.Object);

        var referralCacheKeys = new CacheKeys(mockHttpContextAccessor.Object);

        string result = referralCacheKeys.ConnectionRequest;

        result.Should().Be($"{sessionId}PR");
    }
}