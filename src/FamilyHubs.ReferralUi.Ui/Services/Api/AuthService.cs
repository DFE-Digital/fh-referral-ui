using FamilyHubs.ReferralUi.Ui.Models;
using System.Text.Json;
using System.Text;

namespace FamilyHubs.ReferralUi.Ui.Services.Api;

public interface IAuthService
{
    Task<AccessTokenModel> Login(string username, string password);
    Task<TokenModel> RefreshToken(TokenModel tokenModel);
    Task RevokeToken(string username);
}
public class AuthService : ApiService, IAuthService
{
    public AuthService(HttpClient client, IConfiguration configuration)
        : base(client)
    {
        
    }

    public async Task<AccessTokenModel> Login(string username, string password)
    {
        var model = new ApiLoginModel
        {
            Username = username,
            Password = password
        };

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_client.BaseAddress + "api/Authenticate/login"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var result = await JsonSerializer.DeserializeAsync<AccessTokenModel>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new AccessTokenModel();

        return result;

    }

    public async Task<TokenModel> RefreshToken(TokenModel tokenModel)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_client.BaseAddress + "api/Authenticate/refresh-token"),
            Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(tokenModel), Encoding.UTF8, "application/json"),
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var result = await JsonSerializer.DeserializeAsync<TokenModel>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new TokenModel();

        return result;

    }

    public async Task RevokeToken(string username)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_client.BaseAddress + $"api/Authenticate/revoke/{username}")
        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
    }
}

public class ApiLoginModel
{
    public string? Username { get; set; }

    public string? Password { get; set; }
}

