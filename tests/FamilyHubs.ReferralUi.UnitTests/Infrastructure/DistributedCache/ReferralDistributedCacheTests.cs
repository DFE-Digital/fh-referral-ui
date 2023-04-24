using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Infrastructure.DistributedCache;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Moq;
using System.Text;

namespace FamilyHubs.ReferralUi.UnitTests.Infrastructure.DistributedCache;

public class ReferralDistributedCacheTests
{
    [Fact]
    public async Task GetProfessionalReferralAsync_WhenCalled_ReturnsProfessionalReferral()
    {
        const string key = "key";

        var mockDistributedCache = new Mock<IDistributedCache>();

        var mockReferralCacheKeys = new Mock<IReferralCacheKeys>();
        mockReferralCacheKeys.SetupGet(k => k.ProfessionalReferral).Returns(key);

        var mockDistributedCacheEntryOptions = new Mock<DistributedCacheEntryOptions>();
        var referralDistributedCache = new ReferralDistributedCache(
            mockDistributedCache.Object,
            mockReferralCacheKeys.Object,
            mockDistributedCacheEntryOptions.Object);
        var expectedProfessionalReferralModel = new ProfessionalReferralModel
        {
            FullName = "FullName"
        };

        var serialized = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(expectedProfessionalReferralModel));

        // Moq doesn't support mocking extension methods, so we have to mock internals of the extension method *ugh*
        mockDistributedCache.Setup(x => x.GetAsync(key, default))
            .ReturnsAsync(serialized);

        ProfessionalReferralModel? result = await referralDistributedCache.GetProfessionalReferralAsync();

        result.Should().BeEquivalentTo(expectedProfessionalReferralModel);
    }

    [Fact]
    public async Task SetProfessionalReferralAsync_WhenCalled_SetsProfessionalReferral()
    {
        const string key = "key";

        var mockDistributedCache = new Mock<IDistributedCache>();
        var mockReferralCacheKeys = new Mock<IReferralCacheKeys>();
        mockReferralCacheKeys.SetupGet(k => k.ProfessionalReferral).Returns(key);

        var mockDistributedCacheEntryOptions = new Mock<DistributedCacheEntryOptions>();
        var referralDistributedCache = new ReferralDistributedCache(
            mockDistributedCache.Object,
            mockReferralCacheKeys.Object,
            mockDistributedCacheEntryOptions.Object);
        var professionalReferralModel = new ProfessionalReferralModel
        {
            FullName = "FullName"
        };

        var serialized = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(professionalReferralModel));

        // act
        await referralDistributedCache.SetProfessionalReferralAsync(professionalReferralModel);

        mockDistributedCache.Verify(
            x => x.SetAsync(key, serialized, It.IsAny<DistributedCacheEntryOptions>(), default),
            Times.Once);
    }
}