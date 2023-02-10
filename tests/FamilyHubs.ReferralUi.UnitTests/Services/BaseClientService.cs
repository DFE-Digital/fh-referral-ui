using FamilyHubs.ServiceDirectory.Shared.Builders;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using Moq;
using Moq.Protected;
using System.Net;

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
        var bristolCountyCouncil = new OrganisationWithServicesDto(
            "56e62852-1b0b-40e5-ac97-54a67ea957dc",
            new(string.Empty, string.Empty, string.Empty),
            "Unit Test County Council",
            "Unit Test County Council",
            null,
            new Uri("https://www.unittest.gov.uk/").ToString(),
            "https://www.unittest.gov.uk/",
            new List<ServiceDto>
            {
                 GetTestCountyCouncilServicesDto("56e62852-1b0b-40e5-ac97-54a67ea957dc")
            }
            );

        return bristolCountyCouncil;
    }

    protected static ServiceDto GetTestCountyCouncilServicesDto(string parentId)
    {
        var contactId = Guid.NewGuid().ToString();

        ServicesDtoBuilder builder = new ServicesDtoBuilder();
        ServiceDto service = builder.WithMainProperties(
                id: "3010521b-6e0a-41b0-b610-200edbbeeb14",
                serviceType: new ServiceTypeDto("1", "Information Sharing", ""),
                organisationId: parentId,
                name: "Unit Test Service",
                description: @"Unit Test Service Description",
                accreditations: null,
                assuredDate: null,
                attendingAccess: null,
                attendingType: null,
                deliverableType: null,
                status: "active",
                fees: null,
                canFamilyChooseDeliveryLocation: false)
            .WithServiceDelivery(new List<ServiceDeliveryDto>
                {
                    new ServiceDeliveryDto(Guid.NewGuid().ToString(),ServiceDeliveryType.Online)
                })
            .WithEligibility(new List<EligibilityDto>
                {
                    new EligibilityDto("Test9111Children","",0,13)
                })
            .WithLinkContact(new List<LinkContactDto>
            {
                new LinkContactDto("L1", "3010521b-6e0a-41b0-b610-200edbbeeb14", "Service", 
                new ContactDto(
                    contactId,
                    "Contact",
                    string.Empty,
                    "01827 65777",
                    "01827 65777",
                    "url",
                    "Email"))
            })
            .WithCostOption(new List<CostOptionDto>())
            .WithLanguages(new List<LanguageDto>()
                {
                    new LanguageDto("1bb6c313-648d-4226-9e96-b7d37eaeb3dd", "English")
                })
            .WithServiceAreas(new List<ServiceAreaDto>()
                {
                    new ServiceAreaDto(Guid.NewGuid().ToString(), "National", null,"http://statistics.data.gov.uk/id/statistical-geography/K02000001")
                })
            .WithServiceAtLocations(new List<ServiceAtLocationDto>()
                {
                    new ServiceAtLocationDto(
                        "Test1749",
                        new LocationDto(
                            "6ea31a4f-7dcc-4350-9fba-20525efe092f",
                            "",
                            "",
                            52.6312,
                            -1.66526,
                            new List<PhysicalAddressDto>()
                            {
                                new PhysicalAddressDto(
                                    Guid.NewGuid().ToString(),
                                    "77 Sheepcote Lane",
                                    ", Stathe, Tamworth, Staffordshire, ",
                                    "B77 3JN",
                                    "England",
                                    null
                                    )
                            },
                            new List<LinkTaxonomyDto>(),
                            new List<LinkContactDto>()
                            //new List<Accessibility_For_Disabilities>()
                            ),
                            new List<RegularScheduleDto>(),
                            new List<HolidayScheduleDto>(),
                            new List<LinkContactDto>()

                        )

                })
            .WithServiceTaxonomies(new List<ServiceTaxonomyDto>()
                {
                    new ServiceTaxonomyDto
                    ("UnitTest9107",
                    new TaxonomyDto(
                        "UnitTest bccsource:Organisation",
                        "Organisation",
                        "Test BCC Data Sources",
                        null
                        )),

                    new ServiceTaxonomyDto
                    ("UnitTest9108",
                    new TaxonomyDto(
                        "UnitTest bccprimaryservicetype:38",
                        "Support",
                        "Test BCC Primary Services",
                        null
                        )),

                    new ServiceTaxonomyDto
                    ("UnitTest9109",
                    new TaxonomyDto(
                        "UnitTest bccagegroup:37",
                        "Children",
                        "Test BCC Age Groups",
                        null
                        )),

                    new ServiceTaxonomyDto
                    ("UnitTest9110",
                    new TaxonomyDto(
                        "UnitTestbccusergroup:56",
                        "Long Term Health Conditions",
                        "Test BCC User Groups",
                        null
                        ))
                })
            .Build();

        return service;
    }

    public static ReferralDto GetReferralDto()
    {
        const string JsonService = "{\"Id\":\"ba1cca90-b02a-4a0b-afa0-d8aed1083c0d\",\"Name\":\"Test County Council\",\"Description\":\"Test County Council\",\"Logo\":null,\"Uri\":\"https://www.test.gov.uk/\",\"Url\":\"https://www.test.gov.uk/\",\"Services\":[{\"Id\":\"c1b5dd80-7506-4424-9711-fe175fa13eb8\",\"Name\":\"Test Organisation for Children with Tracheostomies\",\"Description\":\"Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.\",\"Accreditations\":null,\"Assured_date\":null,\"Attending_access\":null,\"Attending_type\":null,\"Deliverable_type\":null,\"Status\":\"active\",\"Url\":\"www.testservice.com\",\"Email\":\"support@testservice.com\",\"Fees\":null,\"ServiceDelivery\":[{\"Id\":\"14db2aef-9292-4afc-be09-5f6f43765938\",\"ServiceDelivery\":2}],\"Eligibilities\":[{\"Id\":\"Test9109Children\",\"Eligibility\":\"\",\"Maximum_age\":0,\"Minimum_age\":13}],\"Contacts\":[{\"Id\":\"5eac5cb6-cc7e-444d-a29b-76ccb85be866\",\"Title\":\"Service\",\"Name\":\"\",\"Phones\":[{\"Id\":\"1568\",\"Number\":\"01827 65779\"}]}],\"Cost_options\":[],\"Languages\":[{\"Id\":\"442a06cd-aa14-4ea3-9f11-b45c1bc4861f\",\"Language\":\"English\"}],\"Service_areas\":[{\"Id\":\"68af19cd-bc81-4585-99a2-85a2b0d62a1d\",\"Service_area\":\"National\",\"Extent\":null,\"Uri\":\"http://statistics.data.gov.uk/id/statistical-geography/K02000001\"}],\"Service_at_locations\":[{\"Id\":\"Test1749\",\"Location\":{\"Id\":\"a878aadc-6097-4a0f-b3e1-77fd4511175d\",\"Name\":\"\",\"Description\":\"\",\"Latitude\":52.6312,\"Longitude\":-1.66526,\"Physical_addresses\":[{\"Id\":\"1076aaa8-f99d-4395-8e4f-c0dde8095085\",\"Address_1\":\"75 Sheepcote Lane\",\"City\":\", Stathe, Tamworth, Staffordshire, \",\"Postal_code\":\"B77 3JN\",\"Country\":\"England\",\"State_province\":null}]}}],\"Service_taxonomys\":[{\"Id\":\"Test9107\",\"Taxonomy\":{\"Id\":\"Test bccsource:Organisation\",\"Name\":\"Organisation\",\"Vocabulary\":\"Test BCC Data Sources\",\"Parent\":null}},{\"Id\":\"Test9108\",\"Taxonomy\":{\"Id\":\"Test bccprimaryservicetype:38\",\"Name\":\"Support\",\"Vocabulary\":\"Test BCC Primary Services\",\"Parent\":null}},{\"Id\":\"Test9109\",\"Taxonomy\":{\"Id\":\"Test bccagegroup:37\",\"Name\":\"Children\",\"Vocabulary\":\"Test BCC Age Groups\",\"Parent\":null}},{\"Id\":\"Test9110\",\"Taxonomy\":{\"Id\":\"Testbccusergroup:56\",\"Name\":\"Long Term Health Conditions\",\"Vocabulary\":\"Test BCC User Groups\",\"Parent\":null}}]}]}";

        return new ReferralDto(
                "5c267ba1-bd82-4919-8cfd-f8622fa9bf9b",
                "ba1cca90-b02a-4a0b-afa0-d8aed1083c0d",
                "c1b5dd80-7506-4424-9711-fe175fa13eb8",
                "Test Organisation for Children with Tracheostomies",
                "Test Organisation for for Children with Tracheostomies is a national self help group operating as a registered charity and is run by parents of children with a tracheostomy and by people who sympathise with the needs of such families. ACT as an organisation is non profit making, it links groups and individual members throughout Great Britain and Northern Ireland.",
                JsonService,
                "Bob Referrer",
                "Mr Robert Brown",
                "No",
                "Robert.Brown@yahoo.co.uk",
                "0131 222 3333",
                "text",
                "Requires help with child",
                null,
                new List<ReferralStatusDto> { new ReferralStatusDto("1d2c41ac-fade-4933-a810-d8a040f0b9ee", "Inital-Referral") }
                );
    }
}
