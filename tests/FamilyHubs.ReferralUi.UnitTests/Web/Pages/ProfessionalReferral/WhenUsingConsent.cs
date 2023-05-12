using FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class WhenUsingConsent
{
    private readonly ConsentModel _consentModel;

    //todo: either move this down into a base or change the cut not to use the UrlHelper?
    private Mock<IUrlHelper> CreateMockUrlHelper(ActionContext? context = null)
    {
        context ??= GetActionContextForPage("/Page");

        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.SetupGet(h => h.ActionContext)
            .Returns(context);
        return urlHelper;
    }

    private static ActionContext GetActionContextForPage(string page)
    {
        return new()
        {
            ActionDescriptor = new()
            {
                RouteValues = new Dictionary<string, string?>
                {
                    { "page", page }
                }
            },
            RouteData = new()
            {
                Values =
                {
                    [ "page" ] = page
                }
            }
        };
    }

    public WhenUsingConsent()
    {
        var mockUrlHelper = CreateMockUrlHelper();
        mockUrlHelper.Setup(h => h.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns("callbackUrl");

        _consentModel = new ConsentModel
        {
            Url = mockUrlHelper.Object
        };
    }

    [Fact]
    public async Task ThenOnGetConsent()
    {
        //Act
        await _consentModel.OnGetAsync("Id");

        _consentModel.ServiceId.Should().Be("Id");
    }

    [Fact]
    public async Task ThenOnGetConsent_With_IsConsentGiven_NotSelected()
    {
        //Arrange
        _consentModel.Consent = default!;

        //Act
        await _consentModel.OnPostAsync("Id");

        //Assert
        _consentModel.ValidationValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("yes", "/ProfessionalReferral/SupportDetails")]
    [InlineData("no", "/ProfessionalReferral/ConsentShutter")]
    public async Task ThenOnGetConsent_With_IsImmediateHarm_Selected(string isConsentGiven, string pageName)
    {
        //Arrange
        _consentModel.Consent = isConsentGiven;

        //Act
        var result = await _consentModel.OnPostAsync("Id") as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be(pageName);
    }
}
