﻿using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FamilyHubs.ReferralUi.UnitTests.Services;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingLocalOfferDetail
{
    public Mock<IReferralDistributedCache> MockReferralDistributedCache;

    public WhenUsingLocalOfferDetail()
    {
        MockReferralDistributedCache = new Mock<IReferralDistributedCache>();
    }

    [Theory]
    [InlineData(default!)]
    [InlineData("url")]
    [InlineData("https://wwww.google.com")]
    [InlineData("http://google.com")]
    public async Task ThenOnGetAsync_LocalOfferDetailWithReferralNotEnabled(string url)
    {
        //Arrange
        Mock<IOrganisationClientService> mockIOrganisationClientService = new Mock<IOrganisationClientService>();
        ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(1);
        if (serviceDto != null && serviceDto.Contacts != null)
        {
            foreach (var linkcontact in serviceDto.Contacts)
            {
                linkcontact.Url = url;
            }
        }

        if (serviceDto != null)
            mockIOrganisationClientService.Setup(x => x.GetLocalOfferById(It.IsAny<string>())).ReturnsAsync(serviceDto);

        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockIOrganisationClientService.Object, MockReferralDistributedCache.Object);
        DefaultHttpContext httpContext = new DefaultHttpContext
        {
            Request =
            {
                Scheme = "http",
                Host = new HostString("localhost"),
                Headers =
                {
                    ["Referer"] = "Referer"
                }
            }
        };
        localOfferDetailModel.PageContext.HttpContext = httpContext;

        //Act 
        var result = await localOfferDetailModel.OnGetAsync((serviceDto != null) ? serviceDto.Id.ToString() : string.Empty) as PageResult;

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PageResult>();
        if (url == null || url == "url")
        {
            localOfferDetailModel.Website.Should().BeEquivalentTo("");
        }
        else
            localOfferDetailModel.Website.Should().BeEquivalentTo(url);
    }

    [Fact]
    public async Task ThenOnGetAsync_WithNullServiceAtLocation()
    {
        //Arrange
        Mock<IOrganisationClientService> mockIOrganisationClientService = new Mock<IOrganisationClientService>();
        ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(1);
        List<ServiceDeliveryDto> deliveryDtoList = new List<ServiceDeliveryDto>(serviceDto.ServiceDeliveries ?? new List<ServiceDeliveryDto>());
        deliveryDtoList.Add(new ServiceDeliveryDto { Id = 1, Name = ServiceDeliveryType.Online, ServiceId = 1 });
        serviceDto.ServiceDeliveries = deliveryDtoList;
        serviceDto.Contacts = default!;
        mockIOrganisationClientService.Setup(x => x.GetLocalOfferById(It.IsAny<string>())).ReturnsAsync(serviceDto);

        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockIOrganisationClientService.Object, MockReferralDistributedCache.Object);
        DefaultHttpContext httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");
        httpContext.Request.Headers["Referer"] = "Referer";
        localOfferDetailModel.PageContext.HttpContext = httpContext;

        //Act 
        var result = await localOfferDetailModel.OnGetAsync(serviceDto.Id.ToString()) as PageResult;

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PageResult>();
        localOfferDetailModel.Email.Should().BeNullOrEmpty();
        localOfferDetailModel.Phone.Should().BeNullOrEmpty();
        localOfferDetailModel.Website.Should().BeNullOrEmpty();

    }

    [Fact]
    public async Task ThenOnGetAsync_WithServiceAtLocationContainingInPerson()
    {
        //Arrange
        Mock<IOrganisationClientService> mockIOrganisationClientService = new Mock<IOrganisationClientService>();
        ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(1);
        mockIOrganisationClientService.Setup(x => x.GetLocalOfferById(It.IsAny<string>())).ReturnsAsync(serviceDto);

        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockIOrganisationClientService.Object, MockReferralDistributedCache.Object);
        DefaultHttpContext httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");
        httpContext.Request.Headers["Referer"] = "Referer";
        localOfferDetailModel.PageContext.HttpContext = httpContext;

        //Act 
        var result = await localOfferDetailModel.OnGetAsync(serviceDto.Id.ToString()) as PageResult;

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PageResult>();
        localOfferDetailModel.Phone.Should().Be("01827 65777");
        localOfferDetailModel.Website.Should().Be("https://www.google.com");
        localOfferDetailModel.Email.Should().Be("Contact@email.com");

    }

    [Fact]
    public async Task ThenOnGetAsync_LocalOfferDetailWithReferralEnabled()
    {
        //Arrange
        Mock<IOrganisationClientService> mockIOrganisationClientService = new Mock<IOrganisationClientService>();
        ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(1);
        mockIOrganisationClientService.Setup(x => x.GetLocalOfferById(It.IsAny<string>())).ReturnsAsync(serviceDto);

        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockIOrganisationClientService.Object, MockReferralDistributedCache.Object);
        DefaultHttpContext httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");
        httpContext.Request.Headers["Referer"] = "Referer";
        localOfferDetailModel.PageContext.HttpContext = httpContext;

        //Act 
        var result = await localOfferDetailModel.OnGetAsync(serviceDto.Id.ToString()) as PageResult;

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PageResult>();
        localOfferDetailModel.Phone.Should().Be("01827 65777");
        localOfferDetailModel.Website.Should().Be("https://www.google.com");
        localOfferDetailModel.Email.Should().Be("Contact@email.com");
    }

    [Fact]
    public void ThenOnPostAsync_ReturnsRedirectToPageResult()
    {
        //Arrange
        Mock<IOrganisationClientService> mockIOrganisationClientService = new Mock<IOrganisationClientService>();
        ServiceDto serviceDto = BaseClientService.GetTestCountyCouncilServicesDto(1);

        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockIOrganisationClientService.Object, MockReferralDistributedCache.Object);

        //Act 
        var result = localOfferDetailModel.OnPost("NewId", serviceDto.Id.ToString(), serviceDto.Name) as RedirectToPageResult;

        //Assert
        result.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be("/ProfessionalReferral/Safeguarding");
    }

    [Fact]
    public void ThenGetDeliveryMethodsAsString_WithNullCollection()
    {
        //Arrange
        Mock<IOrganisationClientService> mockIOrganisationClientService = new Mock<IOrganisationClientService>();

        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockIOrganisationClientService.Object, MockReferralDistributedCache.Object);

        //Act
        string result = localOfferDetailModel.GetDeliveryMethodsAsString(default!);

        //Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ThenGetLanguagesAsString_WithNullCollection()
    {
        //Arrange
        Mock<IOrganisationClientService> mockIOrganisationClientService = new Mock<IOrganisationClientService>();
        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockIOrganisationClientService.Object, MockReferralDistributedCache.Object);

        //Act
        string result = localOfferDetailModel.GetLanguagesAsString(default!);

        //Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void ThenGetLanguagesAsString_ShouldReturnLanguages()
    {
        //Arrange
        Mock<IOrganisationClientService> mockIOrganisationClientService = new Mock<IOrganisationClientService>();
        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockIOrganisationClientService.Object, MockReferralDistributedCache.Object);
        List<LanguageDto> languageDtos = new List<LanguageDto>
        {
            new() { Id = 1, Name = "English", ServiceId = 1 },
            new() { Id = 2, Name = "French", ServiceId = 1 }
        };

        //Act
        string result = localOfferDetailModel.GetLanguagesAsString(languageDtos);

        //Assert
        result.Should().Be("English,French");

    }

    [Fact]
    public void ThenExtractAddressParts_ShouldJustReturn()
    {
        //Arrange
        Mock<IOrganisationClientService> mockIOrganisationClientService = new Mock<IOrganisationClientService>();
        LocalOfferDetailModel localOfferDetailModel = new LocalOfferDetailModel(mockIOrganisationClientService.Object, MockReferralDistributedCache.Object);
        LocationDto locationDto = new LocationDto { Address1 = default!, Address2 = default!, City = default!, Country = default!, Latitude = default!, Longitude = default!, LocationType = LocationType.NotSet, Name = default!, PostCode = default!, StateProvince = default! };

        //Act
        localOfferDetailModel.ExtractAddressParts(locationDto);

        //Assert
        localOfferDetailModel.Address1.Should().BeNullOrEmpty();
        localOfferDetailModel.PostalCode.Should().BeNullOrEmpty();
    }
}
