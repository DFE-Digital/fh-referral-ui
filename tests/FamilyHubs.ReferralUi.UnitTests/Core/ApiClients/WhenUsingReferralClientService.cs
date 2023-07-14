using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.SharedKernel.Security;
using FluentAssertions;
using Moq;
using Moq.Protected;
using System.Net;

namespace FamilyHubs.ReferralUi.UnitTests.Core.ApiClients;

public class WhenUsingReferralClientService
{
    private readonly ReferralDto _referralDto;
    private readonly Mock<ICrypto> _cryptoMock;
    private HttpClient? _httpClient;
    private ReferralClientService? _referralClientService;

    public WhenUsingReferralClientService()
    {
        _referralDto = ClientHelper.GetReferralDto();
        _cryptoMock = new Mock<ICrypto>();
        _cryptoMock.Setup(c => c.EncryptData(It.IsAny<string>())).Returns((string s) => Task.FromResult("encrypted_" + s));
    }

    [Fact]
    public async Task CreateReferral_WithValidData_ReturnsReferralId()
    {
        // Arrange
        _httpClient = ClientHelper.GetMockClient<long>(123);
        _referralClientService = new ReferralClientService(_httpClient, _cryptoMock.Object);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer token");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        // Act
        var result = await _referralClientService.CreateReferral(_referralDto);

        // Assert
        result.Should().Be(123);
        _cryptoMock.Verify(c => c.EncryptData(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CreateReferral_WithInvalidData_ThrowsReferralClientServiceException()
    {
        // Arrange
        _httpClient = new HttpClient();
        _referralClientService = new ReferralClientService(_httpClient, _cryptoMock.Object);

        var responseContent = "Invalid request";

        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(responseContent)
            });

        _httpClient.BaseAddress = new Uri("http://example.com");
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer token");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        _httpClient = new HttpClient(httpMessageHandlerMock.Object);

        // Act and Assert
        await Assert.ThrowsAsync<ReferralClientServiceException>(() => _referralClientService.CreateReferral(_referralDto));
        _cryptoMock.Verify(c => c.EncryptData(It.IsAny<string>()), Times.Exactly(2));
        
    }

}
