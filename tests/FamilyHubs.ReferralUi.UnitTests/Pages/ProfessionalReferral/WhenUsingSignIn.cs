using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FamilyHubs.ReferralUi.Ui.Services;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FamilyHubs.ReferralUi.UnitTests.Pages.ProfessionalReferral;

public class WhenUsingSignIn
{
    private readonly Mock<ICacheService> _mockICacheService;
    internal readonly Ui.Models.ConnectWizzardViewModel _connectWizzardViewModel;
    public WhenUsingSignIn()
    {
        _connectWizzardViewModel = new Ui.Models.ConnectWizzardViewModel
        {
            ServiceId = "ServiceId",
            ServiceName = "ServiceName",
            ReferralId = "ReferralId",
            AnyoneInFamilyBeingHarmed = false,
            HaveConcent = true,
            FullName = "FullName",
            EmailAddress = "someone@email.com",
            Telephone = "01212223333",
            Textphone = "0712345678",
            ReasonForSupport = "Reason For Support"
        };

        _mockICacheService = new Mock<ICacheService>(MockBehavior.Strict);
        _mockICacheService.Setup(x => x.GetUserKey()).Returns("UserKey");
        _mockICacheService.Setup(x => x.RetrieveConnectWizzardViewModel(It.IsAny<string>())).Returns(_connectWizzardViewModel);
        _mockICacheService.Setup(x => x.StoreConnectWizzardViewModel(It.IsAny<string>(), It.IsAny<Ui.Models.ConnectWizzardViewModel>()));
        _mockICacheService.Setup(x => x.ResetConnectWizzardViewModel(It.IsAny<string>()));
    }


    [Theory]
    [InlineData("Professional", "/ProfessionalReferral/FamilyContact")]
    [InlineData("VCSAdmin", "/ProfessionalReferral/Search")]
    [InlineData("", "/ProfessionalReferral/Search")]
    public async Task ThenSignInAsProfessional(string role, string pageName)
    {
        object item = default!;
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.FromResult(item));

        var services = new ServiceCollection();
        services.AddSingleton<IAuthenticationService>(authServiceMock.Object);

        Mock<IAuthService> mockAuthenticationService = new Mock<IAuthService>();
        Mock<ITokenService> mockTokenService = new Mock<ITokenService>();
        JwtSecurityToken jwtSecurityToken = CreateTokenWithRole(role);
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        AccessTokenModel accessTokenModel = new()
        {
            Token = token
        };

        mockAuthenticationService.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(accessTokenModel);
        mockTokenService.Setup(x => x.SetToken(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>()));

        SignInModel signInModel = new SignInModel(mockAuthenticationService.Object, mockTokenService.Object, _mockICacheService.Object);
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        GetClaimsForRole(role).ToArray(), 
        "mock"));
        signInModel.PageContext.HttpContext = new DefaultHttpContext() 
        { 
            RequestServices = services.BuildServiceProvider(),
            User = user 
        };

        //Act
        var result = await signInModel.OnPost("Id", "ServiceName") as RedirectToPageResult;

        //Assert
        ArgumentNullException.ThrowIfNull(result);
        result.PageName.Should().Be(pageName);

    }

    [Fact]
    public void ThenSignInWithException()
    {
        //Arrange
        Mock<IAuthService> mockAuthenticationService = new Mock<IAuthService>();
        Mock<ITokenService> mockTokenService = new Mock<ITokenService>();
        JwtSecurityToken jwtSecurityToken = CreateTokenWithRole("Other");
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        AccessTokenModel accessTokenModel = new()
        {
            Token = token
        };

        mockAuthenticationService.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(accessTokenModel);
        mockTokenService.Setup(x => x.SetToken(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>()));

        SignInModel signInModel = new SignInModel(mockAuthenticationService.Object, mockTokenService.Object, _mockICacheService.Object);
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        GetClaimsForRole("Other").ToArray(),
        "mock"));
        signInModel.PageContext.HttpContext = new DefaultHttpContext()
        {
            User = user
        };


        //Act
        Func<Task> act = async () => { await signInModel.OnPost("Id", "ServiceName"); };
        

        //Assert
        act.Should().ThrowAsync<InvalidOperationException>();
        


    }

    private List<Claim> GetClaimsForRole(string role)
    {
        return new List<Claim>
        {
                    new Claim("UserId", "123"),
                    new Claim(ClaimTypes.Name, "TestUser"),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("OpenReferralOrganisationId", "2d2124ea-3bb0-4802-b694-db02db5e7756"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
    }

    private JwtSecurityToken CreateTokenWithRole(string role)
    {
        var authClaims = GetClaimsForRole(role);

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"));

        var token = new JwtSecurityToken(
            issuer: "https://localhost:7108",
            audience: "MySuperSecureApiUser",
            expires: DateTime.Now.AddMinutes(5),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        return token;
    }
}
