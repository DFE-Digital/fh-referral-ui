using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.ReferralService.Shared.Dto;
using FluentAssertions;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace FamilyHubs.ReferralUi.UnitTests.Core.ApiClients;

public class WhenUsingIdamsClient
{
    [Fact]
    public async Task ThenGetAccountList()
    {
        //Arrange
        var expectedListAccounts = ClientHelper.GetAccountList();
        var jsonString = JsonSerializer.Serialize(expectedListAccounts);

        HttpClient httpClient = ClientHelper.GetMockClient<string>(jsonString);
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer token");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        Mock<IHttpClientFactory> mockClientFactory = new Mock<IHttpClientFactory>();
        mockClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        IIdamsClient idamClientService = new IdamsClient(mockClientFactory.Object);

        //Act
        var result = await idamClientService.GetVcsProfessionalsEmailsAsync(1);

        //Assert
        result.Should().BeEquivalentTo(expectedListAccounts.Select(x => x.Email));
    }

    [Fact]
    public async Task ThenThrowsIdamsClientException()
    {
        // Arrange
        HttpClient httpClient = ClientHelper.GetMockClient<string>("Error message", true);
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer token");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        Mock<IHttpClientFactory> mockClientFactory = new Mock<IHttpClientFactory>();
        mockClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        IIdamsClient idamClientService = new IdamsClient(mockClientFactory.Object);

        // Act and Assert
        await Assert.ThrowsAsync<IdamsClientException>(() => idamClientService.GetVcsProfessionalsEmailsAsync(1, CancellationToken.None));
    }

    /*

    [Fact]
    public async Task ThenThrowsIdamsClientException()
    {
        // Arrange
        HttpClient httpClient = ClientHelper.GetMockClient<string>("Error message", true);
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer token");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        Mock<IHttpClientFactory> mockClientFactory = new Mock<IHttpClientFactory>();
        mockClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        IIdamsClient idamClientService = new IdamsClient(mockClientFactory.Object);

        // Act and Assert
        await Assert.ThrowsAsync<IdamsClientException>(() => idamClientService.GetVcsProfessionalsEmailsAsync(1, CancellationToken.None));


        

    }
    */
}
