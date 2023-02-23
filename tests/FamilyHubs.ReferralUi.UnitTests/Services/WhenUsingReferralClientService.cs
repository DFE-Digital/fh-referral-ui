using FamilyHubs.ReferralUi.Ui.Infrastructure.Configuration;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.SharedKernel;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;

namespace FamilyHubs.ReferralUi.UnitTests.Services;

public class WhenUsingReferralClientService : BaseClientService
{
    [Fact]
    public async Task ThenCreateReferral()
    {
        //Arrange
        var referral = GetReferralDto();
        var mockClient = GetMockClient(referral.Id);
        var mockCfg = new Mock<IOptions<ApiOptions>>();
        mockCfg.Setup(x => x.Value).Returns(new ApiOptions());
        ReferralClientService referralClientService = new(mockClient, mockCfg.Object);

        //Act
        var result = await referralClientService.CreateReferral(referral);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(referral.Id);

    }

    [Fact]
    public async Task ThenGetReferralById()
    {
        //Arrange
        var referral = GetReferralDto();
        var json = JsonConvert.SerializeObject(referral);
        var mockClient = GetMockClient(json);
        var mockCfg = new Mock<IOptions<ApiOptions>>();
        mockCfg.Setup(x => x.Value).Returns(new ApiOptions());
        ReferralClientService referralClientService = new(mockClient, mockCfg.Object);

        //Act
        var result = await referralClientService.GetReferralById(referral.Id);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(referral);

    }

    [Fact]
    public async Task ThenGetReferralsByOrganisationId()
    {
        //Arrange
        var referral = new PaginatedList<ReferralDto>(new List<ReferralDto> { GetReferralDto() }, 1, 1, 1);
        var json = JsonConvert.SerializeObject(referral);
        var mockClient = GetMockClient(json);
        var mockCfg = new Mock<IOptions<ApiOptions>>();
        mockCfg.Setup(x => x.Value).Returns(new ApiOptions());
        ReferralClientService referralClientService = new(mockClient, mockCfg.Object);

        //Act
        var result = await referralClientService.GetReferralsByOrganisationId(referral.Items[0].Id,1,1);

        //Assert
        result.Should().NotBeNull();
        result.Items[0].Id.Should().BeEquivalentTo(referral.Items[0].Id);

    }

    [Fact]
    public async Task ThenGetReferralsByReferrer()
    {
        //Arrange
        var referral = new PaginatedList<ReferralDto>(new List<ReferralDto> { GetReferralDto() }, 1, 1, 1);
        var json = JsonConvert.SerializeObject(referral);
        var mockClient = GetMockClient(json);
        var mockCfg = new Mock<IOptions<ApiOptions>>();
        mockCfg.Setup(x => x.Value).Returns(new ApiOptions());
        ReferralClientService referralClientService = new(mockClient, mockCfg.Object);

        //Act
        var result = await referralClientService.GetReferralsByReferrer(referral.Items[0].Referrer, 1, 1);

        //Assert
        result.Should().NotBeNull();
        result.Items[0].Referrer.Should().BeEquivalentTo(referral.Items[0].Referrer);

    }

    [Fact]
    public async Task ThenUpdateReferral()
    {
        //Arrange
        var referral = GetReferralDto();
        var mockClient = GetMockClient(referral.Id);
        var mockCfg = new Mock<IOptions<ApiOptions>>();
        mockCfg.Setup(x => x.Value).Returns(new ApiOptions());
        ReferralClientService referralClientService = new(mockClient, mockCfg.Object);

        //Act
        var result = await referralClientService.UpdateReferral(referral);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(referral.Id);

    }

    [Fact]
    public async Task ThenSetReferralStatus()
    {
        //Arrange
        var referral = GetReferralDto();
        var mockClient = GetMockClient(referral.Id);
        var mockCfg = new Mock<IOptions<ApiOptions>>();
        mockCfg.Setup(x => x.Value).Returns(new ApiOptions());
        ReferralClientService referralClientService = new(mockClient, mockCfg.Object);

        //Act
        var result = await referralClientService.SetReferralStatusReferral(referral.Id, "Test Status");

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(referral.Id);

    }
}
