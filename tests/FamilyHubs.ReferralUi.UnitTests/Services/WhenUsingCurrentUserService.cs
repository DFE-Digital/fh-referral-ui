using FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;
using FamilyHubs.ReferralUi.Ui.Services;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FamilyHubs.ReferralUi.UnitTests.Services;

public class WhenUsingCurrentUserService
{
    [Fact]
    public void ThenGetUserId()
    {
        //Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "VCS Admin"),
            new Claim(ClaimTypes.NameIdentifier, "Name Of User"),
            new Claim(ClaimTypes.Role, "VCSAdmin"),
            new Claim("OpenReferralOrganisationId", Guid.NewGuid().ToString()),
        }, "mock"));

        var httpContext = new DefaultHttpContext() { User = user };
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        CurrentUserService currentUserService = new CurrentUserService(mockHttpContextAccessor.Object);

        //Act
        var result = currentUserService.UserId;

        //Assert
        result.Should().Be("Name Of User");

    }
}
