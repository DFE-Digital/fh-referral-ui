using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class LocalOfferResultsPage
{
    private LocalOfferResultsModel pageModel;

    public LocalOfferResultsPage()
    {
        var mockLocalOfferClientService = new Mock<ILocalOfferClientService>();
        var mockIPostcodeLocationClientService = new Mock<IPostcodeLocationClientService>();
        var mockIOpenReferralOrganisationClientService = new Mock<IOpenReferralOrganisationClientService>();


        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("IsReferralEnabled", "false")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        pageModel = new LocalOfferResultsModel(mockLocalOfferClientService.Object, mockIPostcodeLocationClientService.Object, mockIOpenReferralOrganisationClientService.Object, configuration);
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
