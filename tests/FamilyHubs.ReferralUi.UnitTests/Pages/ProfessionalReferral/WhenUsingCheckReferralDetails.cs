using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Builders;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FluentAssertions;
using FluentAssertions.Common;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingCheckReferralDetails : BaseProfessionalReferralPage
{
    private readonly CheckReferralDetailsModel _checkReferralDetailsModel;

    
    private readonly Mock<ILocalOfferClientService> _localOfferClientService;
    private readonly Mock<IReferralClientService> _referralClientService;

    public WhenUsingCheckReferralDetails()
    {
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("UseRabbitMQ", "False")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        
        _localOfferClientService = new Mock<ILocalOfferClientService>();
        _referralClientService = new Mock<IReferralClientService>();
        _localOfferClientService.Setup(x => x.GetLocalOfferById(It.IsAny<string>())).ReturnsAsync(GetTestCountyCouncilServicesDto("56e62852-1b0b-40e5-ac97-54a67ea957dc"));

        _referralClientService.Setup(x => x.CreateReferral(It.IsAny<ReferralDto>())).ReturnsAsync(_connectWizzardViewModel.ReferralId);
        _referralClientService.Setup(x => x.UpdateReferral(It.IsAny<ReferralDto>())).ReturnsAsync(_connectWizzardViewModel.ReferralId);

        _checkReferralDetailsModel = new CheckReferralDetailsModel(configuration, _localOfferClientService.Object, _referralClientService.Object,  _mockIRedisCacheService.Object);
    }

    [Fact]
    public void ThenOnGetCheckReferralDetails()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);

        //Act
        _checkReferralDetailsModel.OnGet();

        //Assert
        _checkReferralDetailsModel.Id.Should().Be(_connectWizzardViewModel.ServiceId);
        _checkReferralDetailsModel.Name.Should().Be(_connectWizzardViewModel.ServiceName);
        _checkReferralDetailsModel.ReferralId.Should().Be(_connectWizzardViewModel.ReferralId);
        _checkReferralDetailsModel.FullName.Should().Be(_connectWizzardViewModel.FullName);
        _checkReferralDetailsModel.Email.Should().Be(_connectWizzardViewModel.EmailAddress);
        _checkReferralDetailsModel.Telephone.Should().Be(_connectWizzardViewModel.Telephone);
        _checkReferralDetailsModel.Textphone.Should().Be(_connectWizzardViewModel.Textphone);
        _checkReferralDetailsModel.ReasonForSupport.Should().Be(_connectWizzardViewModel.ReasonForSupport);
        
    }

    [Fact]
    public async Task ThenOnPostCheckReferralDetails()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);


        //Act
        var result = await _checkReferralDetailsModel.OnPost() as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/ConfirmReferral");
    }

    [Fact]
    public async Task ThenOnPostCheckReferralDetails_Updates()
    {
        //Arrange
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _referralClientService.Setup(x => x.GetReferralById(It.IsAny<string>())).ReturnsAsync(new ReferralDto(
            id: _connectWizzardViewModel.ReferralId,
            organisationId: "56e62852-1b0b-40e5-ac97-54a67ea957dc",
            serviceId: _connectWizzardViewModel.ServiceId,
            serviceName: _connectWizzardViewModel.ServiceName,
            serviceDescription: _connectWizzardViewModel.ServiceName,
            serviceAsJson: string.Empty, 
            referrer: string.Empty,
            fullName: string.Empty,
            hasSpecialNeeds: "no",
            email: default!,
            phone: default!,
            text: default!,
            reasonForSupport: string.Empty,
            reasonForRejection: string.Empty,
            new List<ReferralStatusDto>()));

        //Act
        var result = await _checkReferralDetailsModel.OnPost() as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/ConfirmReferral");
    }

    [Fact]
    public async Task ThenOnPostCheckReferralDetails_WithMissingReferralId()
    {
        //Arrange
        Ui.Models.ConnectWizzardViewModel model = new Ui.Models.ConnectWizzardViewModel
        {
            ServiceId = "ServiceId",
            ServiceName = "ServiceName",
            AnyoneInFamilyBeingHarmed = false,
            HaveConcent = true,
            FullName = "FullName",
            EmailAddress = "someone@email.com",
            Telephone = "01212223333",
            Textphone = "0712345678",
            ReasonForSupport = "Reason For Support"
        };
        _mockIRedisCacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(model);
        _referralClientService.Setup(x => x.GetReferralById(It.IsAny<string>())).ReturnsAsync(new ReferralDto(
            id: _connectWizzardViewModel.ReferralId,
            organisationId: "56e62852-1b0b-40e5-ac97-54a67ea957dc",
            serviceId: _connectWizzardViewModel.ServiceId,
            serviceName: _connectWizzardViewModel.ServiceName,
            serviceDescription: _connectWizzardViewModel.ServiceName,
            serviceAsJson: string.Empty,
            referrer: string.Empty,
            fullName: string.Empty,
            hasSpecialNeeds: "no",
            email: default!,
            phone: default!,
            text: default!,
            reasonForSupport: string.Empty,
            reasonForRejection: string.Empty,
            new List<ReferralStatusDto>()));

        //Act
        var result = await _checkReferralDetailsModel.OnPost() as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/ConfirmReferral");
    }

    public OrganisationWithServicesDto GetTestCountyCouncilDto()
    {
        var testCountyCouncil = new OrganisationWithServicesDto(
            "56e62852-1b0b-40e5-ac97-54a67ea957dc", 
            new OrganisationTypeDto("1", "LA", "Local Authority"),
            "Unit Test County Council",
            "Unit Test County Council",
            null,
            new Uri("https://www.unittest.gov.uk/").ToString(),
            "https://www.unittest.gov.uk/",
            new List<ServiceDto>
            {
                GetTestCountyCouncilServicesDto("56e62852-1b0b-40e5-ac97-54a67ea957dc")
            }
        )
        {
            AdminAreaCode = "XTEST"
        };

        return testCountyCouncil;
    }

    public ServiceDto GetTestCountyCouncilServicesDto(string parentId)
    {
        var contactId = Guid.NewGuid().ToString();

        var builder = new ServicesDtoBuilder();
        var service = builder.WithMainProperties(_connectWizzardViewModel.ServiceId,
                new ServiceTypeDto("1", "Information Sharing", ""),
                parentId,
                "Unit Test Service",
                @"Unit Test Service Description",
                "accreditations",
                DateTime.Now,
                "attending access",
                "attending type",
                "delivery type",
                "active",
                null,
                false)
            .WithServiceDelivery(new List<ServiceDeliveryDto>
            {
                new ServiceDeliveryDto("77cc3815-6b95-4618-ab27-ac9f35c44614",ServiceDeliveryType.Online)
            })
            .WithEligibility(new List<EligibilityDto>
            {
                new EligibilityDto("Test9111Children","",1,13)
            })
            .WithLinkContact(new List<LinkContactDto>
            {
                new LinkContactDto(
                    "3010521b-6e0a-41b0-b610-200edbbeeb11",
                    "3010521b-6e0a-41b0-b610-200edbbeeb14",
                    "Service",
                    new ContactDto(
                        contactId,
                        "Contact",
                        string.Empty,
                        "01827 65777",
                        "01827 65777",
                        "www.unittestservice.com",
                        "support@unittestservice.com"
                    ))
            })
            .WithCostOption(new List<CostOptionDto> {  new CostOptionDto(
                "22001144-26d5-4dcc-a6a5-62ce2ce98cc0",
                "amount_description1",
                decimal.Zero,
                default!,
                "free",
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(8))
            })
            .WithLanguages(new List<LanguageDto>
            {
                new LanguageDto("1bb6c313-648d-4226-9e96-b7d37eaeb312", "English")
            })
            .WithServiceAreas(new List<ServiceAreaDto>
            {
                new ServiceAreaDto("a302aea4-fe0c-4ccc-9178-bea39f3edc30", "National", null,"http://statistics.data.gov.uk/id/statistical-geography/K02000001")
            })
            .WithServiceAtLocations(new List<ServiceAtLocationDto>
            {
                new ServiceAtLocationDto(
                    "Test1749",
                    new LocationDto(
                        "6ea31a4f-7dcc-4350-9fba-20525efe092f",
                        "Test Location",
                        "",
                        52.6312,
                        -1.66526,
                        new List<PhysicalAddressDto>
                        {
                            new PhysicalAddressDto(
                                "c576191d-9f14-4963-885e-2889a7a2b48f",
                                "77 Sheepcote Lane",
                                ", Stathe, Tamworth, Staffordshire, ",
                                "B77 3JN",
                                "England",
                                null
                            )
                        },
                        new List<LinkTaxonomyDto>
                        {
                            new LinkTaxonomyDto(
                                "d53b3524-bd3e-443c-ae14-69482afc7d2a",
                                "Location",
                                "6ea31a4f-7dcc-4350-9fba-20525efe092f",
                                new TaxonomyDto(
                                    "b60b7f3e-9ff4-48b2-bded-b00272ed3aba",
                                    "Family_hub 1",
                                    null,
                                    null
                                )
                            )
                        },
                        new List<LinkContactDto>
                        {
                            new LinkContactDto(
                                "3010521b-6e0a-41b0-b610-200edbbeeb33",
                                "6ea31a4f-7dcc-4350-9fba-20525efe092f",
                                "Service",
                                new ContactDto(
                                    Guid.NewGuid().ToString(),
                                    "Contact",
                                    string.Empty,
                                    "01827 65777",
                                    "01827 65777",
                                    "www.unittestservice.com",
                                    "support@unittestservice.com"
                                ))
                        }
                    ),
                    new List<RegularScheduleDto>
                    {
                        new RegularScheduleDto(
                            "5e5ba093-a5f9-49ce-826c-52851e626288",
                            "Description",
                            DateTime.UtcNow,
                            DateTime.UtcNow.AddHours(8),
                            "byDay1",
                            "byMonth",
                            "dtStart",
                            "freq",
                            "interval",
                            DateTime.UtcNow,
                            DateTime.UtcNow.AddMonths(6)
                        )
                    },
                    new List<HolidayScheduleDto>
                    {
                        new HolidayScheduleDto(
                            "bc946512-7f8c-4c54-b7ed-ad8fefde7b48",
                            false,
                            DateTime.UtcNow,
                            DateTime.UtcNow,
                            DateTime.UtcNow.AddDays(5) ,
                            DateTime.UtcNow
                        )
                    },
                    new List<LinkContactDto>
                    {
                        new LinkContactDto(
                            "Test17491234",
                            "Test1749",
                            "ServiceAtLocation",
                            new ContactDto(
                                Guid.NewGuid().ToString(),
                                "Contact",
                                string.Empty,
                                "01827 65777",
                                "01827 65777",
                                "www.unittestservice.com",
                                "support@unittestservice.com"
                            ))
                    }
                )

            })
            .WithServiceTaxonomies(new List<ServiceTaxonomyDto>
            {
                new ServiceTaxonomyDto(
                    "UnitTest9107",
                    new TaxonomyDto(
                        "UnitTest bccsource:Organisation",
                        "Organisation",
                        "Test BCC Data Sources",
                        null
                    )),

                new ServiceTaxonomyDto(
                    "UnitTest9108",
                    new TaxonomyDto(
                        "UnitTest bccprimaryservicetype:38",
                        "Support",
                        "Test BCC Primary Services",
                        null
                    )),

                new ServiceTaxonomyDto(
                    "UnitTest9109",
                    new TaxonomyDto(
                        "UnitTest bccagegroup:37",
                        "Children",
                        "Test BCC Age Groups",
                        null
                    )),

                new ServiceTaxonomyDto(
                    "UnitTest9110",
                    new TaxonomyDto(
                        "UnitTestbccusergroup:56",
                        "Long Term Health Conditions",
                        "Test BCC User Groups",
                        null
                    ))
            })
            .WithFundings(new List<FundingDto>())
            .Build();

        service.RegularSchedules = new List<RegularScheduleDto>();
        service.HolidaySchedules = new List<HolidayScheduleDto>();

        return service;
    }
}
