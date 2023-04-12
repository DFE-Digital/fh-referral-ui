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

    public static ServiceDto GetTestCountyCouncilServicesDto(long parentId)
    {
        var serviceId = "9066bccb-79cb-401f-818f-86ad23b022cf";

        var service = new ServiceDto
        {
            OrganisationId = 1,
            ServiceOwnerReferenceId = serviceId,
            ServiceType = ServiceType.InformationSharing,
            Name = "Test Organisation for Children with Tracheostomies",
            Description = @"Test1 Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.",
            ServiceDeliveries = new List<ServiceDeliveryDto>
            {
                new ServiceDeliveryDto
                {
                    Name = ServiceDeliveryType.Online,
                }
            },
            Eligibilities = new List<EligibilityDto>
            {
                new EligibilityDto
                {
                    MaximumAge = 13,
                    MinimumAge = 0
                }
            },
            Contacts = new List<ContactDto>
            {
                new ContactDto
                {
                    Name =  "Service",
                    Title = string.Empty,
                    Telephone = "01827 65770",
                    TextPhone = "01827 65770",
                    Url = "www.testservice1.com",
                    Email = "support@testservice1.com"
                }
            },
            Languages = new List<LanguageDto>
            {
                new LanguageDto
                {
                    Name = "English"
                }
            },
            ServiceAreas = new List<ServiceAreaDto>
            {
                new ServiceAreaDto
                {
                    ServiceAreaName = "National",
                    Extent = null,
                    Uri = "http://statistics.data.gov.uk/id/statistical-geography/K02000001",
                }
            },
            Locations = new List<LocationDto>
            {
                new LocationDto
                {
                    Name = "Test",
                    Description = "",
                    Latitude = 52.6312,
                    Longitude = -1.66526,
                    Address1 = "76 Sheepcote Lane",
                    City = ", Stathe, Tamworth, Staffordshire, ",
                    PostCode = "B77 3JN",
                    Country = "England",
                    StateProvince = "null",
                    LocationType = LocationType.FamilyHub
                }
            }
        };

        return service;
    }
}
