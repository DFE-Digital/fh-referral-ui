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
    public const string Key = "key";
    public Mock<IDistributedCache> MockDistributedCache;
    public Mock<IReferralCacheKeys> MockReferralCacheKeys;
    public Mock<DistributedCacheEntryOptions> MockDistributedCacheEntryOptions;
    public ReferralDistributedCache ReferralDistributedCache;
    public ProfessionalReferralModel ProfessionalReferralModel;
    public byte[] ProfessionalReferralModelSerializedBytes;

    public ReferralDistributedCacheTests()
    {
        MockDistributedCache = new Mock<IDistributedCache>();

        MockReferralCacheKeys = new Mock<IReferralCacheKeys>();
        MockReferralCacheKeys.SetupGet(k => k.ProfessionalReferral).Returns(Key);

        MockDistributedCacheEntryOptions = new Mock<DistributedCacheEntryOptions>();
        ReferralDistributedCache = new ReferralDistributedCache(
            MockDistributedCache.Object,
            MockReferralCacheKeys.Object,
            MockDistributedCacheEntryOptions.Object);
        ProfessionalReferralModel = new ProfessionalReferralModel
        {
            FullName = "FullName",
            ServiceId = "1",
            ServiceName = "ServiceName"
        };

        ProfessionalReferralModelSerializedBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ProfessionalReferralModel));
    }

    [Fact]
    public async Task GetProfessionalReferralAsync_WhenCalled_ReturnsProfessionalReferral()
    {
        // Moq doesn't support mocking extension methods, so we have to mock internals of the extension method *ugh*
        MockDistributedCache.Setup(x => x.GetAsync(Key, default))
            .ReturnsAsync(ProfessionalReferralModelSerializedBytes);

        // act
        ProfessionalReferralModel? result = await ReferralDistributedCache.GetProfessionalReferralAsync();

        result.Should().BeEquivalentTo(ProfessionalReferralModel);
    }

    [Fact]
    public async Task SetProfessionalReferralAsync_WhenCalled_SetsProfessionalReferral()
    {
        // act
        await ReferralDistributedCache.SetProfessionalReferralAsync(ProfessionalReferralModel);

        MockDistributedCache.Verify(
            x => x.SetAsync(Key, ProfessionalReferralModelSerializedBytes, It.IsAny<DistributedCacheEntryOptions>(), default),
            Times.Once);
    }
}