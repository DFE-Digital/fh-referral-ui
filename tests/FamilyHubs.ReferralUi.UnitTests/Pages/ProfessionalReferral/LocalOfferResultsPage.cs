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
}
