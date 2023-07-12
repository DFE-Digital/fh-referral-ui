using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
}
