using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.Services;
using FamilyHubs.Referral.Infrastructure;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FamilyHubs.ReferralUi.UnitTests;

public class WhenUsingDummyProfessional
{
    [Fact]
    public void ThenGetProfessionalUserToken()
    {
        //Arrange and Act
        string result = DummyProfessional.GetProfessionalUserToken();
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(result);
        var claims = jwtSecurityToken.Claims.ToList();
        var idClaim = claims.FirstOrDefault(x => x.Type == "OpenReferralOrganisationId");
        var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

        //Assert
        ArgumentNullException.ThrowIfNull(idClaim);
        ArgumentNullException.ThrowIfNull(emailClaim);
        idClaim.Value.Should().Be("1");
        emailClaim.Value.Should().Be("BtlPro@email.com");
    }

    [Fact]
    public async Task ThenLogInProfessionalUser()
    {
        //Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        new List<Claim>
        {
                    new Claim("BtlPro", "123"),
                    new Claim(ClaimTypes.Email, "BtlPro@email.com"),
                    new Claim(ClaimTypes.Name, "Bristol Professional"),
                    new Claim(ClaimTypes.Role, "Professional"),
                    new Claim("OpenReferralOrganisationId", "1"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        }.ToArray(),
       "mock"));

        object item = default!;
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.FromResult(item));

        var services = new ServiceCollection();
        services.AddSingleton<IAuthenticationService>(authServiceMock.Object);

        var httpContext = new DefaultHttpContext()
        {
            RequestServices = services.BuildServiceProvider(),
            User = user
        };

        Mock<IDistributedCacheService> mockRedisCacheService = new Mock<IDistributedCacheService>();
        int setCallback = 0;
        mockRedisCacheService.Setup(x => x.StoreStringValue(It.IsAny<string>(), It.IsAny<string>())).Callback(() => setCallback++);
        TokenService tokenService = new TokenService(mockRedisCacheService.Object);


        //Act
        long result = await DummyProfessional.LogUserIn(httpContext, tokenService, user, DummyProfessional.GetProfessionalUserToken());


        //Assert
        result.Should().Be(1);
        setCallback.Should().Be(2);
        
    }

    [Fact]
    public async Task ThenLogoutProfessionalUser()
    {
        //Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        new List<Claim>
        {
                    new Claim("BtlPro", "123"),
                    new Claim(ClaimTypes.Email, "BtlPro@email.com"),
                    new Claim(ClaimTypes.Name, "Bristol Professional"),
                    new Claim(ClaimTypes.Role, "Professional"),
                    new Claim("OpenReferralOrganisationId", "1"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        }.ToArray(),
       "mock"));

        object item = default!;
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.FromResult(item));

        var services = new ServiceCollection();
        services.AddSingleton<IAuthenticationService>(authServiceMock.Object);

        var httpContext = new DefaultHttpContext()
        {
            RequestServices = services.BuildServiceProvider(),
            User = user
        };

        Mock<IDistributedCacheService> mockRedisCacheService = new Mock<IDistributedCacheService>();
        int setCallback = 0;
        mockRedisCacheService.Setup(x => x.StoreStringValue(It.IsAny<string>(), It.IsAny<string>())).Callback(() => setCallback++);
        TokenService tokenService = new TokenService(mockRedisCacheService.Object);
        long result = await DummyProfessional.LogUserIn(httpContext, tokenService, user, DummyProfessional.GetProfessionalUserToken());

        //Act
        await DummyProfessional.LogUserOut(httpContext, tokenService);



        //Assert
        result.Should().Be(1);
        setCallback.Should().Be(4);

    }
}
