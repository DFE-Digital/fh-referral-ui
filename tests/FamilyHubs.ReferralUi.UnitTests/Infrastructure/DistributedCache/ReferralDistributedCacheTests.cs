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
    public const string ProfessionalsEmail = "pro@example.com";
    public Mock<IDistributedCache> MockDistributedCache;
    public Mock<DistributedCacheEntryOptions> MockDistributedCacheEntryOptions;
    public ConnectionRequestDistributedCache ConnectionRequestDistributedCache;
    public ConnectionRequestModel ConnectionRequestModel;
    public byte[] ProfessionalReferralModelSerializedBytes;

    public ReferralDistributedCacheTests()
    {
        MockDistributedCache = new Mock<IDistributedCache>();

        MockDistributedCacheEntryOptions = new Mock<DistributedCacheEntryOptions>();
        ConnectionRequestDistributedCache = new ConnectionRequestDistributedCache(
            MockDistributedCache.Object,
            MockDistributedCacheEntryOptions.Object);
        ConnectionRequestModel = new ConnectionRequestModel
        {
            FamilyContactFullName = "FamilyContactFullName",
            ServiceId = "1"
        };

        ProfessionalReferralModelSerializedBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ConnectionRequestModel));
    }

    [Fact]
    public async Task GetProfessionalReferralAsync_WhenCalled_ReturnsProfessionalReferral()
    {
        // Moq doesn't support mocking extension methods, so we have to mock internals of the extension method *ugh*
        MockDistributedCache.Setup(x => x.GetAsync(ProfessionalsEmail, default))
            .ReturnsAsync(ProfessionalReferralModelSerializedBytes);

        // act
        ConnectionRequestModel? result = await ConnectionRequestDistributedCache.GetAsync(ProfessionalsEmail);

        result.Should().BeEquivalentTo(ConnectionRequestModel);
    }

    [Fact]
    public async Task SetProfessionalReferralAsync_WhenCalled_SetsProfessionalReferral()
    {
        // act
        await ConnectionRequestDistributedCache.SetAsync(ProfessionalsEmail, ConnectionRequestModel);

        MockDistributedCache.Verify(
            x => x.SetAsync(ProfessionalsEmail, ProfessionalReferralModelSerializedBytes, It.IsAny<DistributedCacheEntryOptions>(), default),
            Times.Once);
    }

    [Fact]
    public async Task RemoveProfessionalReferralAsync_WhenCalled_RemovesProfessionalReferral()
    {
        // act
        await ConnectionRequestDistributedCache.RemoveAsync(ProfessionalsEmail);
        MockDistributedCache.Verify(x => x.RemoveAsync(ProfessionalsEmail, default), Times.Once);
    }
}