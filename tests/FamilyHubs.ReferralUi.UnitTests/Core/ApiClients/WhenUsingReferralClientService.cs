﻿using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.SharedKernel.Security;
using FluentAssertions;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using FamilyHubs.ReferralService.Shared.Models;

namespace FamilyHubs.ReferralUi.UnitTests.Core.ApiClients;

public class WhenUsingReferralClientService
{
    private readonly ReferralDto _referralDto;
    private HttpClient? _httpClient;
    private ReferralClientService? _referralClientService;

    public WhenUsingReferralClientService()
    {
        _referralDto = ClientHelper.GetReferralDto();
    }

    [Fact]
    public async Task CreateReferral_WithValidData_ReturnsReferralId()
    {
        // Arrange
        var jsonString = JsonSerializer.Serialize(new ReferralResponse
        {
            Id = 123,
            ServiceName = "At your service",
            OrganisationId = 456
        });

        _httpClient = ClientHelper.GetMockClient(jsonString);
        _referralClientService = new ReferralClientService(_httpClient);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer token");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        // Act
        var result = await _referralClientService.CreateReferral(_referralDto);

        // Assert
        result.Id.Should().Be(123);
    }

    [Fact]
    public async Task CreateReferral_WithInvalidData_ThrowsReferralClientServiceException()
    {
        // Arrange
        _httpClient = new HttpClient();
        _referralClientService = new ReferralClientService(_httpClient);

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
    }

}
