﻿using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System.Text.Json;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingSearch
{
    [Fact]
    public async Task OnPost_WhenPostcodeIsValid_ThenValidationShouldBeTrue()
    {
        //Arrange
        string json = @"{
    ""status"": 200,
    ""result"": {
        ""postcode"": ""BS2 0SP"",
        ""quality"": 1,
        ""eastings"": 361195,
        ""northings"": 172262,
        ""country"": ""England"",
        ""nhs_ha"": ""South West"",
        ""longitude"": -2.559788,
        ""latitude"": 51.448006,
        ""european_electoral_region"": ""South West"",
        ""primary_care_trust"": ""Bristol"",
        ""region"": ""South West"",
        ""lsoa"": ""Bristol 056B"",
        ""msoa"": ""Bristol 056"",
        ""incode"": ""0SP"",
        ""outcode"": ""BS2"",
        ""parliamentary_constituency"": ""Bristol West"",
        ""admin_district"": ""Bristol, City of"",
        ""parish"": ""Bristol, City of, unparished area"",
        ""admin_county"": null,
        ""date_of_introduction"": ""199412"",
        ""admin_ward"": ""Lawrence Hill"",
        ""ced"": null,
        ""ccg"": ""NHS Bristol, North Somerset and South Gloucestershire"",
        ""nuts"": ""Bristol, City of"",
        ""pfa"": ""Avon and Somerset"",
        ""codes"": {
            ""admin_district"": ""E06000023"",
            ""admin_county"": ""E99999999"",
            ""admin_ward"": ""E05010907"",
            ""parish"": ""E43000019"",
            ""parliamentary_constituency"": ""E14000602"",
            ""ccg"": ""E38000222"",
            ""ccg_id"": ""15C"",
            ""ced"": ""E99999999"",
            ""nuts"": ""TLK11"",
            ""lsoa"": ""E01014658"",
            ""msoa"": ""E02006889"",
            ""lau2"": ""E06000023"",
            ""pfa"": ""E23000036""
            }
        }
    }";

        PostcodesIoResponse postcodesIoResponse = JsonSerializer.Deserialize<PostcodesIoResponse>(json) ?? new PostcodesIoResponse();
        var mockPostcodeLocationCLientService = new Mock<IPostcodeLocationClientService>();
        mockPostcodeLocationCLientService
            .Setup(action => action.LookupPostcode(It.IsAny<string>()))
            .ReturnsAsync(postcodesIoResponse);
        SearchModel searchModel = new SearchModel(mockPostcodeLocationCLientService.Object);

        //Act
        var onPostResult = await searchModel.OnPost() as PageResult;

        //Assert
        Assert.True(searchModel.ValidationValid);
    }

    [Fact]
    public async Task OnPost_WhenPostcodeIsNotValid_ThenValidationShouldBeFalse()
    {
        //Arrange
        string json = @"{
    ""status"": 200,
    ""result"": {
        ""postcode"": ""BS2 0SP"",
        ""quality"": 1,
        ""eastings"": 361195,
        ""northings"": 172262,
        ""country"": ""England"",
        ""nhs_ha"": ""South West"",
        ""longitude"": -2.559788,
        ""latitude"": 51.448006,
        ""european_electoral_region"": ""South West"",
        ""primary_care_trust"": ""Bristol"",
        ""region"": ""South West"",
        ""lsoa"": ""Bristol 056B"",
        ""msoa"": ""Bristol 056"",
        ""incode"": ""0SP"",
        ""outcode"": ""BS2"",
        ""parliamentary_constituency"": ""Bristol West"",
        ""admin_district"": ""Bristol, City of"",
        ""parish"": ""Bristol, City of, unparished area"",
        ""admin_county"": null,
        ""date_of_introduction"": ""199412"",
        ""admin_ward"": ""Lawrence Hill"",
        ""ced"": null,
        ""ccg"": ""NHS Bristol, North Somerset and South Gloucestershire"",
        ""nuts"": ""Bristol, City of"",
        ""pfa"": ""Avon and Somerset"",
        ""codes"": {
            ""admin_district"": ""E06000023"",
            ""admin_county"": ""E99999999"",
            ""admin_ward"": ""E05010907"",
            ""parish"": ""E43000019"",
            ""parliamentary_constituency"": ""E14000602"",
            ""ccg"": ""E38000222"",
            ""ccg_id"": ""15C"",
            ""ced"": ""E99999999"",
            ""nuts"": ""TLK11"",
            ""lsoa"": ""E01014658"",
            ""msoa"": ""E02006889"",
            ""lau2"": ""E06000023"",
            ""pfa"": ""E23000036""
            }
        }
    }";

        PostcodesIoResponse postcodesIoResponse = JsonSerializer.Deserialize<PostcodesIoResponse>(json) ?? new PostcodesIoResponse();
        var mockPostcodeLocationCLientService = new Mock<IPostcodeLocationClientService>();
        mockPostcodeLocationCLientService
            .Setup(action => action.LookupPostcode(It.IsAny<string>()))
            .ReturnsAsync(postcodesIoResponse);
        SearchModel searchModel = new SearchModel(mockPostcodeLocationCLientService.Object);

        //Act
        searchModel.Postcode = "aaa";
        var onPostResult = await searchModel.OnPost() as PageResult;

        //Assert
        Assert.False(searchModel.ValidationValid);
    }
}
