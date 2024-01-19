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
        ServiceDto service = new()
        {
            Id = 1,
            OrganisationId = parentId,
            ServiceType = ServiceType.FamilyExperience,
            Name = "Unit Test Service",
            Description = "Unit Test Service Description",
            Accreditations = null,
            AssuredDate = DateTime.Now,
            AttendingAccess = AttendingAccessType.NotSet,
            AttendingType = AttendingType.NotSet,
            DeliverableType = DeliverableType.NotSet,
            Status = ServiceStatusType.Active,
            Fees = null,
            CanFamilyChooseDeliveryLocation = false,
            ServiceOwnerReferenceId = "e4f18752-f4f9-4476-9d4f-a9644c599a53",
            ServiceAreas = new List<ServiceAreaDto>()
            {
                new ServiceAreaDto
                {
                    Id = 2,
                    ServiceAreaName = "National",
                    Uri = "http://statistics.data.gov.uk/id/statistical-geography/K02000001",
                    ServiceId = 1,

                }
            },
            Contacts = new List<ContactDto>()
            {
                new ContactDto
                {
                    Id = 3,
                    ServiceId = 1,
                    Title = "Mr",
                    Name = "Contact",
                    Email = "Contact@email.com",
                    Telephone = "01827 65777",
                    TextPhone = "01827 65777",
                    Url = "www.google.com"
                }
            },
            Eligibilities = new List<EligibilityDto>()
            {
                new EligibilityDto
                {
                    Id = 4,
                    ServiceId = 1,
                    EligibilityType = null,
                    MinimumAge = 1,
                    MaximumAge = 13,
                }
            },
            CostOptions = new List<CostOptionDto>(),
            Languages = new List<LanguageDto>()
            {
                new LanguageDto
                {
                    Id = 5,
                    ServiceId = 1,
                    Name = "English",
                    Code = "en"
                }
            },
            ServiceDeliveries = new List<ServiceDeliveryDto>(),
            Schedules = new List<ScheduleDto>(),
            Locations = new List<LocationDto>()
            {
                new LocationDto
                {
                    Id = 6,
                    Schedules = new List<ScheduleDto>(),
                    Name = "Shepcoat",
                    Address1 = "77 Sheepcote Lane",
                    Address2 = "Stathe",
                    City = "Tamworth",
                    PostCode = "B77 3JN",
                    StateProvince = "Staffordshire",
                    Country = "England",
                    Latitude = 52.6312,
                    Longitude = -1.66526,
                    LocationType = LocationType.NotSet,
                }
            },
            Fundings = new List<FundingDto>(),
            Taxonomies = new List<TaxonomyDto>()
            {
                new TaxonomyDto
                {
                    Id = 7,
                    Name = "UnitTest bccprimaryservicetype:38",
                    TaxonomyType = TaxonomyType.ServiceCategory,

                }
            }
        };

        return service;
    }
}
