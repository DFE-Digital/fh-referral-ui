using FamilyHubs.Referral.Core.ApiClients;
using FluentAssertions;

namespace FamilyHubs.ReferralUi.UnitTests.Services;

public class WhenUsingPostcodeLocationClientService : BaseClientService
{
    [Fact]
    public async Task ThenLookupPostcode()
    {
        //Arrange
        var json = @"{
    ""status"": 200,
    ""result"": {
        ""postcode"": ""BS14 0AL"",
        ""quality"": 1,
        ""eastings"": 359880,
        ""northings"": 168841,
        ""country"": ""England"",
        ""nhs_ha"": ""South West"",
        ""longitude"": -2.578321,
        ""latitude"": 51.417154,
        ""european_electoral_region"": ""South West"",
        ""primary_care_trust"": ""Bristol"",
        ""region"": ""South West"",
        ""lsoa"": ""Bristol 047A"",
        ""msoa"": ""Bristol 047"",
        ""incode"": ""0AL"",
        ""outcode"": ""BS14"",
        ""parliamentary_constituency"": ""Bristol South"",
        ""admin_district"": ""Bristol, City of"",
        ""parish"": ""Bristol, City of, unparished area"",
        ""admin_county"": null,
        ""date_of_introduction"": ""198001"",
        ""admin_ward"": ""Hengrove & Whitchurch Park"",
        ""ced"": null,
        ""ccg"": ""NHS Bristol, North Somerset and South Gloucestershire"",
        ""nuts"": ""Bristol, City of"",
        ""pfa"": ""Avon and Somerset"",
        ""codes"": {
            ""admin_district"": ""E06000023"",
            ""admin_county"": ""E99999999"",
            ""admin_ward"": ""E05010902"",
            ""parish"": ""E43000019"",
            ""parliamentary_constituency"": ""E14000601"",
            ""ccg"": ""E38000222"",
            ""ccg_id"": ""15C"",
            ""ced"": ""E99999999"",
            ""nuts"": ""TLK11"",
            ""lsoa"": ""E01014607"",
            ""msoa"": ""E02003058"",
            ""lau2"": ""E06000023"",
            ""pfa"": ""E23000036""
        }
    }
}";
        var mockClient = GetMockClient(json);
        var postcodeLocationClientService = new PostcodeLocationClientService(mockClient);

        //Act
        var result = await postcodeLocationClientService.LookupPostcode("BS14 0AL");

        //Assert
        result.Should().NotBeNull();
        result.Result.Postcode.Should().Be("BS14 0AL");

    }
}
