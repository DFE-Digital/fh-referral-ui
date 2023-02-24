using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FamilyHubs.ReferralUi.UnitTests.Services;

public class WhenUsingAuthService : BaseClientService
{
    [Fact]
    public async Task ThenLoginWithAuthService()
    {
        //Arrange
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("IsReferralEnabled", "true")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        AccessTokenModel accessTokenModel = new AccessTokenModel
        {
            Token = "TestToken",
            RefreshToken = "RefreshToken",
            Expiration = DateTime.Now
        };
        var json = JsonConvert.SerializeObject(accessTokenModel);
        var mockClient = GetMockClient(json);
        AuthService authService = new(mockClient, configuration);

        //Act
        var result = await authService.Login("username","password");

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(accessTokenModel);
    }

    [Fact]
    public async Task ThenRefreshTokenWithAuthService()
    {
        //Arrange
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("IsReferralEnabled", "true")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        TokenModel tokenModel = new TokenModel
        {
            AccessToken= "TestToken",
            RefreshToken = "RefreshToken"
        };
        var json = JsonConvert.SerializeObject(tokenModel);
        var mockClient = GetMockClient(json);
        AuthService authService = new(mockClient, configuration);

        //Act
        var result = await authService.RefreshToken(tokenModel);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(tokenModel);
    }

    [Fact]
    public void ThenRevokeTokenWithAuthService()
    {
        //Arrange
        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("IsReferralEnabled", "true")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        TokenModel tokenModel = new TokenModel
        {
            AccessToken = "TestToken",
            RefreshToken = "RefreshToken"
        };
        var json = JsonConvert.SerializeObject(tokenModel);
        var mockClient = GetMockClient(json);
        AuthService authService = new(mockClient, configuration);

        //Act
        Func<Task> act = async () => { await authService.RevokeToken("username"); };

        //Assert
        act.Should().NotThrowAsync();
    }
}
