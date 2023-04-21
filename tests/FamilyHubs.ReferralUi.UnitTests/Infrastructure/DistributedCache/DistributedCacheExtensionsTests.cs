using FluentAssertions;
using FamilyHubs.Referral.Infrastructure.DistributedCache;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Infrastructure.DistributedCache;

public class DistributedCacheExtensionsTests
{
    Mock<IDistributedCache> DistributedCache = new();

    [Fact]
    public async Task GetAsync_WhenObjectIsNotInCache_ReturnsNull()
    {
        // Moq doesn't support mocking extension methods, so we have to mock internals of the extension method *ugh*
        DistributedCache.Setup(x => x.GetAsync("key", default))
            .ReturnsAsync((byte[]?)null);

        var result = await DistributedCache.Object.GetAsync<object>("key");
            
        result.Should().Be(null);
    }
}