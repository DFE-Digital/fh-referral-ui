namespace FamilyHubs.Referral.Core.Services;

public interface ITokenService
{
    string GetToken();
    string GetRefreshToken();
    void SetToken(string tokenValue, DateTime validTo, string refreshToken);
    void ClearTokens();
    string GetUsersOrganisationId();
    string GetUserKey();
}
