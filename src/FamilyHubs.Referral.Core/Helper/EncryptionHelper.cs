using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.SharedKernel.Security;

namespace FamilyHubs.Referral.Core.Helper;

public static class EncryptionHelper
{
    public static async Task<ReferralDto> EncrptReferralAsync(this ReferralDto referral, ICrypto crypto)
    {
        referral.ReasonForSupport = await crypto.EncryptData(referral.ReasonForSupport);
        referral.EngageWithFamily = await crypto.EncryptData(referral.EngageWithFamily);

        referral.RecipientDto.Name = !string.IsNullOrEmpty(referral.RecipientDto.Name) ? await crypto.EncryptData(referral.RecipientDto.Name) : referral.RecipientDto.Name;
        referral.RecipientDto.Email = !string.IsNullOrEmpty(referral.RecipientDto.Email) ? await crypto.EncryptData(referral.RecipientDto.Email) : referral.RecipientDto.Email;
        referral.RecipientDto.Telephone = !string.IsNullOrEmpty(referral.RecipientDto.Telephone) ? await crypto.EncryptData(referral.RecipientDto.Telephone) : referral.RecipientDto.Telephone;
        referral.RecipientDto.TextPhone = !string.IsNullOrEmpty(referral.RecipientDto.TextPhone) ? await crypto.EncryptData(referral.RecipientDto.TextPhone) : referral.RecipientDto.TextPhone;
        referral.RecipientDto.AddressLine1 = !string.IsNullOrEmpty(referral.RecipientDto.AddressLine1) ? await crypto.EncryptData(referral.RecipientDto.AddressLine1) : referral.RecipientDto.AddressLine1;
        referral.RecipientDto.AddressLine2 = !string.IsNullOrEmpty(referral.RecipientDto.AddressLine2) ? await crypto.EncryptData(referral.RecipientDto.AddressLine2) : referral.RecipientDto.AddressLine2;
        referral.RecipientDto.TownOrCity = !string.IsNullOrEmpty(referral.RecipientDto.TownOrCity) ? await crypto.EncryptData(referral.RecipientDto.TownOrCity) : referral.RecipientDto.TownOrCity;
        referral.RecipientDto.County = !string.IsNullOrEmpty(referral.RecipientDto.County) ? await crypto.EncryptData(referral.RecipientDto.County) : referral.RecipientDto.County;
        referral.RecipientDto.PostCode = !string.IsNullOrEmpty(referral.RecipientDto.PostCode) ? await crypto.EncryptData(referral.RecipientDto.PostCode) : referral.RecipientDto.PostCode;


        return referral;
    }

    public static async Task<ReferralDto> DecryptReferralAsync(this ReferralDto referral, ICrypto crypto)
    {
        referral.ReasonForSupport = await crypto.DecryptData(referral.ReasonForSupport);
        referral.EngageWithFamily = await crypto.DecryptData(referral.EngageWithFamily);

        referral.RecipientDto.Name = !string.IsNullOrEmpty(referral.RecipientDto.Name) ? await crypto.DecryptData(referral.RecipientDto.Name) : referral.RecipientDto.Name;
        referral.RecipientDto.Email = !string.IsNullOrEmpty(referral.RecipientDto.Email) ? await crypto.DecryptData(referral.RecipientDto.Email) : referral.RecipientDto.Email;
        referral.RecipientDto.Telephone = !string.IsNullOrEmpty(referral.RecipientDto.Telephone) ? await crypto.DecryptData(referral.RecipientDto.Telephone) : referral.RecipientDto.Telephone;
        referral.RecipientDto.TextPhone = !string.IsNullOrEmpty(referral.RecipientDto.TextPhone) ? await crypto.DecryptData(referral.RecipientDto.TextPhone) : referral.RecipientDto.TextPhone;
        referral.RecipientDto.AddressLine1 = !string.IsNullOrEmpty(referral.RecipientDto.AddressLine1) ? await crypto.DecryptData(referral.RecipientDto.AddressLine1) : referral.RecipientDto.AddressLine1;
        referral.RecipientDto.AddressLine2 = !string.IsNullOrEmpty(referral.RecipientDto.AddressLine2) ? await crypto.DecryptData(referral.RecipientDto.AddressLine2) : referral.RecipientDto.AddressLine2;
        referral.RecipientDto.TownOrCity = !string.IsNullOrEmpty(referral.RecipientDto.TownOrCity) ? await crypto.DecryptData(referral.RecipientDto.TownOrCity) : referral.RecipientDto.TownOrCity;
        referral.RecipientDto.County = !string.IsNullOrEmpty(referral.RecipientDto.County) ? await crypto.DecryptData(referral.RecipientDto.County) : referral.RecipientDto.County;
        referral.RecipientDto.PostCode = !string.IsNullOrEmpty(referral.RecipientDto.PostCode) ? await crypto.DecryptData(referral.RecipientDto.PostCode) : referral.RecipientDto.PostCode;


        return referral;
    }
}
