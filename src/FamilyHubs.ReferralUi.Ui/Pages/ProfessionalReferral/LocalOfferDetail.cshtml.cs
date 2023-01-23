using EnumsNET;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralLanguages;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralPhysicalAddresses;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralServiceDeliverysEx;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class LocalOfferDetailModel : PageModel
{
    private readonly ILocalOfferClientService _localOfferClientService;

    public OpenReferralServiceDto LocalOffer { get; set; } = default!;

    public string? ReturnUrl { get; set; }

    public bool IsReferralEnabled { get; private set; }

    [BindProperty]
    public string Name { get; set; } = default!;

    public string Address_1 { get; set; } = default!;
    public string City { get; set; } = default!;
    public string State_province { get; set; } = default!;
    public string Postal_code { get; set; } = default!;
    public string Phone { get; set; } = default!;

    public LocalOfferDetailModel(ILocalOfferClientService localOfferClientService, IConfiguration configuration)
    {
        _localOfferClientService = localOfferClientService;
        IsReferralEnabled = configuration.GetValue<bool>("IsReferralEnabled");
    }

    //Needs to pass dummy id so service id can be any string
    public async Task<IActionResult> OnGetAsync(string id, string serviceid)
    {
        if (IsReferralEnabled)
        {
            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/ProfessionalReferral/SignIn", new
                {
                });
            }
        }

        
        ReturnUrl = Request.Headers["Referer"].ToString();
        LocalOffer = await _localOfferClientService.GetLocalOfferById(serviceid);
        if (LocalOffer != null)
        {
            Name = LocalOffer.Name;
        }
        ExtractAddressParts(LocalOffer?.Service_at_locations?.FirstOrDefault()?.Location?.Physical_addresses?.FirstOrDefault() ?? new OpenReferralPhysicalAddressDto());
        GetTelephone();

        return Page();
    }

    public IActionResult OnPost(string id, string name)
    {
        return RedirectToPage("/ProfessionalReferral/ConnectFamilyToServiceStart", new
        {
            id,
            name
        });

    }


    public string GetDeliveryMethodsAsString(ICollection<OpenReferralServiceDeliveryExDto>? serviceDeliveries)
    {
        var result = string.Empty;

        if (serviceDeliveries == null || serviceDeliveries.Count == 0)
            return result;

        foreach (var serviceDelivery in serviceDeliveries)
        {
            result = result +
                    serviceDelivery.ServiceDelivery.AsString(EnumFormat.Description) != null ?
                    serviceDelivery.ServiceDelivery.AsString(EnumFormat.Description) + "," :
                    String.Empty;
        }

        //Remove last comma if present
        if (result.EndsWith(","))
        {
            result = result.Remove(result.Length - 1);
        }

        return result;
    }

    public string GetLanguagesAsString(ICollection<OpenReferralLanguageDto>? languageDtos)
    {
        var result = string.Empty;

        if (languageDtos == null || languageDtos.Count == 0)
            return result;

        foreach (var language in languageDtos)
            result = result + language.Language + ",";

        //Remove last comma if present
        if (result.EndsWith(","))
        {
            result = result.Remove(result.Length - 1);
        }

        return result;
    }

    public void ExtractAddressParts(OpenReferralPhysicalAddressDto addressDto)
    {
        if (addressDto.Address_1 == string.Empty)
            return;

        Address_1 = (addressDto.Address_1 != null ? addressDto.Address_1 + "," : string.Empty);
        City = (addressDto.City != null ? addressDto.City + "," : string.Empty);
        State_province = (addressDto.State_province != null ? addressDto.State_province + "," : string.Empty);
        Postal_code = (addressDto.Postal_code != null ? addressDto.Postal_code : string.Empty);
    }

    private void GetTelephone()
    {
        if (LocalOffer.Contacts == null)
            return;

        foreach (var contact in LocalOffer.Contacts)
        {
            //Telephone
            if (contact.Name == "Telephone")
            {
                Phone = contact.Telephone;
            }
        }
    }
}
