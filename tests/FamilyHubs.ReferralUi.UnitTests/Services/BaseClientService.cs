using System.Net;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using Moq;
using Moq.Protected;

namespace FamilyHubs.ReferralUi.UnitTests.Services;

public class BaseClientService
{
    protected static HttpClient GetMockClient(string content)
    {
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(content),
                StatusCode = HttpStatusCode.OK
            });

        var client = new HttpClient(mockHttpMessageHandler.Object);
        client.BaseAddress = new Uri("Https://Localhost");
        return client;
    }

    protected OrganisationWithServicesDto GetTestCountyCouncilDto()
    {
        var bristolCountyCouncil = new OrganisationWithServicesDto {
        OrganisationType = OrganisationType.NotSet, Name = "Unit Test County Council", Description = "Unit Test County Council", AdminAreaCode = ""  };
        return bristolCountyCouncil;
    }
}
