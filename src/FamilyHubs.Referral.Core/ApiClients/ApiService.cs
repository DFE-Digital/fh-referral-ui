namespace FamilyHubs.Referral.Core.ApiClients;

public interface IApiService
{

}

public class ApiService : IApiService
{
    protected readonly HttpClient Client;

    public ApiService(HttpClient client)
    {
        Client = client;
    }
}
