using EnumsNET;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class LocalOfferDetailModel : PageModel
{
    private readonly ILocalOfferClientService _localOfferClientService;

    public ServiceDto LocalOffer { get; set; } = default!;

    public string? ReturnUrl { get; set; }

    public bool IsReferralEnabled { get; private set; }

    [BindProperty]
    public string ServiceId { get; set; } = default!;

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

        ServiceId = serviceid;
        ReturnUrl = Request.Headers["Referer"].ToString();
        LocalOffer = await _localOfferClientService.GetLocalOfferById(serviceid);
        Name = LocalOffer.Name;
        ExtractAddressParts(LocalOffer?.ServiceAtLocations?.FirstOrDefault()?.Location?.PhysicalAddresses?.FirstOrDefault() ?? new PhysicalAddressDto());
        GetTelephone();

        return Page();
    }

    public IActionResult OnPost(string id, string serviceId, string name)
    {
        return RedirectToPage("/ProfessionalReferral/ConnectFamilyToServiceStart", new
        {
            id = serviceId,
            name
        });

    }


    public string GetDeliveryMethodsAsString(ICollection<ServiceDeliveryDto>? serviceDeliveries)
    {
        var result = string.Empty;

        if (serviceDeliveries == null || serviceDeliveries.Count == 0)
            return result;

        foreach (var serviceDelivery in serviceDeliveries)
        {
            result = result +
                    serviceDelivery.Name.AsString(EnumFormat.Description) != null ?
                    serviceDelivery.Name.AsString(EnumFormat.Description) + "," :
                    String.Empty;
        }

        //Remove last comma if present
        if (result.EndsWith(","))
        {
            result = result.Remove(result.Length - 1);
        }

        return result;
    }

    public string GetLanguagesAsString(ICollection<LanguageDto>? languageDtos)
    {
        var result = string.Empty;

        if (languageDtos == null || languageDtos.Count == 0)
            return result;

        foreach (var language in languageDtos)
            result = result + language.Name + ",";

        //Remove last comma if present
        if (result.EndsWith(","))
        {
            result = result.Remove(result.Length - 1);
        }

        return result;
    }

    public void ExtractAddressParts(PhysicalAddressDto addressDto)
    {
        if (addressDto.Address1 == string.Empty)
            return;

        Address_1 = addressDto.Address1 + ",";
        City = addressDto.City != null ? addressDto.City + "," : string.Empty;
        State_province = addressDto.StateProvince != null ? addressDto.StateProvince + "," : string.Empty;
        Postal_code = addressDto.PostCode;
    }

    private void GetTelephone()
    {
        if (LocalOffer.LinkContacts == null)
            return;

        //if there are more then one contact with Name equal "Telephone" then bellow code will pick the last record
        foreach (var linkcontact in LocalOffer.LinkContacts)
        {
            //Telephone
            if (linkcontact.Contact.Name == "Telephone")
            {
                Phone = linkcontact.Contact.Telephone;
            }
        }
    }
}
