using FamilyHubs.Referral.Core.ApiClients;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FamilyHubs.ReferralUi.UnitTests.Core.ApiClients;

public class WhenUsingPostcodeLocationClientService
{
    [Fact]
    public async Task ThenLookupPostcode()
    {
        //Arrange
        var postcode = "SW1A 1AA";
        var adminArea = "Greater London";
        var latitude = 51.5033;
        var longitude = -0.1276;
        var outCode = "SW1A";
        var response = ClientHelper.FillPostcodesIoResponse(postcode, adminArea, latitude, longitude, outCode);

        var jsonString = JsonSerializer.Serialize(response);

        HttpClient httpClient = ClientHelper.GetMockClient<string>(jsonString);
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer token");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        IPostcodeLocationClientService postcodeLocationClientService = new PostcodeLocationClientService(httpClient);

        //Act
        var result = await postcodeLocationClientService.LookupPostcode(postcode);

        //Assert
        result.Should().BeEquivalentTo(response);
    }
}
