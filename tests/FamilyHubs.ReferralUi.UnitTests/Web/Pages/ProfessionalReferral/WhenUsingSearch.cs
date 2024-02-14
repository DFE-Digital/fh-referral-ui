using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FamilyHubs.SharedKernel.Services.Postcode.Interfaces;
using FamilyHubs.SharedKernel.Services.Postcode.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingSearch
{
    [Fact]
    public async Task OnPost_WhenPostcodeIsValid_ThenValidationShouldBeTrue()
    {
        //Arrange
        var postcodeLookup = new Mock<IPostcodeLookup>();
        postcodeLookup
            .Setup(action => action.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PostcodeError.None, null));
        var searchModel = new SearchModel(postcodeLookup.Object);

        //Act
        _ = await searchModel.OnPostAsync() as PageResult;

        //Assert
        Assert.True(searchModel.PostcodeValid);
    }

    [Fact]
    public async Task OnPost_WhenPostcodeIsNotValid_ThenValidationShouldBeFalse()
    {
        //Arrange
        var postcodeService = new Mock<IPostcodeLookup>();
        postcodeService
            .Setup(action => action.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PostcodeError.InvalidPostcode, null));
        var searchModel = new SearchModel(postcodeService.Object)
        {
            Postcode = "aaa"
        };

        _ = await searchModel.OnPostAsync() as PageResult;

        //Assert
        Assert.False(searchModel.PostcodeValid);
    }
}
