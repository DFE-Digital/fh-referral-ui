using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.SharedKernel.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Routing;

namespace FamilyHubs.ReferralUi.UnitTests.Web.Pages.ProfessionalReferral;

public class BaseProfessionalReferralPage
{
    public Mock<IConnectionRequestDistributedCache> ReferralDistributedCache;
    public readonly ConnectionRequestModel ConnectionRequestModel;

    public const string Reason = "Reason";
    public const string EmailAddress = "example.com";
    public const string Telephone = "07700 900000";
    public const string Text = "07700 900001";
    public const string AddressLine1 = "AddressLine1";
    public const string AddressLine2 = "AddressLine2";
    public const string TownOrCity = "TownOrCity";
    public const string County = "County";
    public const string Postcode = "Postcode";
    public const string EngageReason = "EngageReason";

    public const string ProfessionalEmail = "Joe.Professional@email.com";

    public BaseProfessionalReferralPage()
    {
        ConnectionRequestModel = new ConnectionRequestModel
        {
            ServiceId = "ServiceId",
            FamilyContactFullName = "FamilyContactFullName",
            Reason = Reason,
            ContactMethodsSelected = new[] { true, true, true, true },
            EmailAddress = EmailAddress,
            TelephoneNumber = Telephone,
            TextphoneNumber = Text,
            AddressLine1 = AddressLine1,
            AddressLine2 = AddressLine2,
            TownOrCity = TownOrCity,
            County = County,
            Postcode = Postcode,
            EngageReason = EngageReason
        };

        ReferralDistributedCache = new Mock<IConnectionRequestDistributedCache>(MockBehavior.Strict);
        //todo: add pro's email to class and check key, rather than It.IsAny<string>()
        ReferralDistributedCache.Setup(x => x.SetAsync(It.IsAny<string>(),It.IsAny<ConnectionRequestModel>())).Returns(Task.CompletedTask);
        ReferralDistributedCache.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(ConnectionRequestModel);
        ReferralDistributedCache.Setup(x => x.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
    }

    protected PageContext GetPageContextWithUserClaims()
    {
        var displayName = "User name";
        var identity = new GenericIdentity(displayName);
        identity.AddClaim(new Claim(FamilyHubsClaimTypes.Role, "Professional"));
        identity.AddClaim(new Claim(FamilyHubsClaimTypes.OrganisationId, "1"));
        identity.AddClaim(new Claim(FamilyHubsClaimTypes.AccountId, "1"));
        identity.AddClaim(new Claim(FamilyHubsClaimTypes.AccountStatus, "active"));
        identity.AddClaim(new Claim(FamilyHubsClaimTypes.FullName, "Test User"));
        identity.AddClaim(new Claim(FamilyHubsClaimTypes.ClaimsValidTillTime, DateTime.UtcNow.AddMinutes(30).ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Email, ProfessionalEmail));
        identity.AddClaim(new Claim(FamilyHubsClaimTypes.PhoneNumber, "012345678"));
        var principle = new ClaimsPrincipal(identity);
        // use default context with user
        var httpContext = new DefaultHttpContext()
        {
            User = principle
        };

        //need these as well for the page context
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
        var modelMetadataProvider = new EmptyModelMetadataProvider();
        var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
        // need page context for the page model
        return new PageContext(actionContext)
        {
            ViewData = viewData
        };
    }
}