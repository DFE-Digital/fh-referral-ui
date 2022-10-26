using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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
    public void NoResultsShouldBeReturned_WhenSearchPostCodeIsNullOrEmpty(string postCode)
    {
        //Arrange
        pageModel.SearchPostCode = postCode;

        // Act
        var searchResults = pageModel.SearchResults;

        // Assert
        searchResults.Should().BeNull();
    }

}
