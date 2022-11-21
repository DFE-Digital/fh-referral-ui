using FamilyHubs.ReferralUi.Ui.Pages;
using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralServices;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class LocalOfferResultsPage
{
    private LocalOfferResultsModel pageModel;

    public LocalOfferResultsPage()
    {
        var mockLocalOfferClientService = new Mock<ILocalOfferClientService>();
        var mockIPostcodeLocationClientService = new Mock<IPostcodeLocationClientService>();
        pageModel = new LocalOfferResultsModel(mockLocalOfferClientService.Object, mockIPostcodeLocationClientService.Object);
    }

    //[Fact]
    //public void OnGetAsync_PopulatesThePageModel_WithAListOfServices()
    //{
    //    throw new NotImplementedException();
    //}

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void OnGetAsync_WhenSearchPostCodeIsNullOrEmpty_ThenNoResultsShouldBeReturned(string postCode)
    {
        // Act
        var searchResults = pageModel.OnGetAsync(postCode, 0.0D, 0.0D, 0.0D, "", "", "") as IActionResult;

        // Assert
        searchResults.Should().BeNull();
    }

    //[Theory]
    //[InlineData("KT22 8RX")]
    //public void OnGetAsync_WhenSearchPostCodeIsValid_ThenResultsShouldBeReturned(string postCode)
    //{
    //    // Act
    //    var searchResults = pageModel.OnGetAsync(postCode, 0.0D, 0.0D, 0.0D, "", "", "") as IActionResult;

    //    // Assert
    //    searchResults.Should().NotBeNull();
    //}

    //[Fact]
    //public void OnPostAsync_WhenSearchingForFreeServices_ThenFreeServicesShouldBeReturned()
    //{
    //    throw new NotImplementedException();
    //}

    //[Fact]
    //public void OnPostAsync_WhenSearchingForFreeServices_ThenPaidServicesShouldNotBeReturned()
    //{
    //    throw new NotImplementedException();
    //}

    //[Fact]
    //public void OnPostAsync_WhenSearchingForPaidServices_ThenPaidServicesShouldBeReturned()
    //{
    //    throw new NotImplementedException();
    //}

    //[Fact]
    //public void OnPostAsync_WhenSearchingForPaidServices_ThenFreeServicesShouldNotBeReturned()
    //{
    //    throw new NotImplementedException();
    //}

}
