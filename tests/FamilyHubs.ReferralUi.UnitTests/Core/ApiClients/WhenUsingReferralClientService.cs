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
        _referralDto = GetReferralDto();
        _cryptoMock = new Mock<ICrypto>();
        _cryptoMock.Setup(c => c.EncryptData(It.IsAny<string>())).Returns((string s) => Task.FromResult("encrypted_" + s));
    }

    [Fact]
    public async Task CreateReferral_WithValidData_ReturnsReferralId()
    {
        // Arrange
        _httpClient = GetMockClient(123);
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

    private HttpClient GetMockClient(int content)
    {
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(content.ToString()),
                StatusCode = HttpStatusCode.OK
            }); 

        var client = new HttpClient(mockHttpMessageHandler.Object);
        client.BaseAddress = new Uri("Https://Localhost");
        return client;
    }

    public static ReferralDto GetReferralDto()
    {
        return new ReferralDto
        {
            Id = 2,
            ReasonForSupport = "Reason For Support",
            EngageWithFamily = "Engage With Family",
            RecipientDto = new RecipientDto
            {
                Id = 2,
                Name = "Joe Blogs",
                Email = "JoeBlog@email.com",
                Telephone = "078123456",
                TextPhone = "078123456",
                AddressLine1 = "Address Line 1",
                AddressLine2 = "Address Line 2",
                TownOrCity = "Town or City",
                County = "County",
                PostCode = "B30 2TV"
            },
            ReferralUserAccountDto = new UserAccountDto
            {
                Id = 2,
                EmailAddress = "Bob.Referrer@email.com",
                Name = "Bob Referrer",
                PhoneNumber = "1234567890",
                Team = "Team",
                UserAccountRoles = new List<UserAccountRoleDto>()
                    {
                        new UserAccountRoleDto
                        {
                            UserAccount = new UserAccountDto
                            {
                                EmailAddress = "Bob.Referrer@email.com",
                            },
                            Role = new RoleDto
                            {
                                Name = "VcsProfessional"
                            }
                        }
                    },
                ServiceUserAccounts = new List<UserAccountServiceDto>(),
                OrganisationUserAccounts = new List<UserAccountOrganisationDto>(),
            },
            Status = new ReferralStatusDto
            {
                Id = 1,
                Name = "New",
                SortOrder = 0
            },
            ReferralServiceDto = new ReferralServiceDto
            {
                Id = 2,
                Name = "Service",
                Description = "Service Description",
                Url = "www.service.com",
                OrganisationDto = new OrganisationDto
                {
                    Id = 2,
                    ReferralServiceId = 2,
                    Name = "Organisation",
                    Description = "Organisation Description",
                }
            }

        };
    }
}
