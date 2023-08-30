using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.SharedKernel.Identity;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

[Authorize]
public class LocalOfferDetailModel : HeaderPageModel
{
    private readonly IOrganisationClientService _organisationClientService;
    public ServiceDto LocalOffer { get; set; } = default!;

    public string? ReturnUrl { get; set; }

    [BindProperty]
    public string ServiceId { get; set; } = default!;

    [BindProperty]
    public string Name { get; set; } = default!;
    public string Address1 { get; set; } = default!;
    public string City { get; set; } = default!;
    public string StateProvince { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Website { get; set; } = default!;
    public string Email { get; set; } = default!;

    public bool ShowConnectionRequestButton { get; set; }

    public LocalOfferDetailModel(IOrganisationClientService organisationClientService)
    {
        _organisationClientService = organisationClientService;
    }

    public async Task<IActionResult> OnGetAsync(string serviceId)
    {
        ServiceId = serviceId;
        var referer = Request.Headers["Referer"];
        ReturnUrl = StringValues.IsNullOrEmpty(referer) ? Url.Page("Search") : referer.ToString();
        LocalOffer = await _organisationClientService.GetLocalOfferById(serviceId);
        Name = LocalOffer.Name;
        if (LocalOffer.Locations != null && LocalOffer.Locations.Any())
        {
            ExtractAddressParts(LocalOffer.Locations.First());
        }
        GetContactDetails();

        ShowConnectionRequestButton = HttpContext.GetRole()is
            RoleTypes.LaProfessional or RoleTypes.LaDualRole;

        return Page();
    }

    public string GetDeliveryMethodsAsString(ICollection<ServiceDeliveryDto>? serviceDeliveries)
    {
        var result = string.Empty;

        if (serviceDeliveries == null || serviceDeliveries.Count == 0)
            return result;

        result = string.Join(',', serviceDeliveries.Select(serviceDelivery => serviceDelivery.Name).ToArray());

        return result;
    }

    //todo: looks like a custom implementation of string.Join
    public string GetLanguagesAsString(ICollection<LanguageDto>? languageDtos)
    {
        var result = string.Empty;

        if (languageDtos == null || languageDtos.Count == 0)
            return result;

        var stringBuilder = new StringBuilder();
        foreach (var language in languageDtos)
            stringBuilder.Append(language.Name + ",");

        result = stringBuilder.ToString();

        //Remove last comma if present
        if (result.EndsWith(','))
        {
            result = result.Remove(result.Length - 1);
        }

        return result;
    }

    public void ExtractAddressParts(LocationDto addressDto)
    {
        if (string.IsNullOrEmpty(addressDto.Address1))
            return;

        Address1 = addressDto.Address1.Replace(",", "") + ",";
        City = !string.IsNullOrWhiteSpace(addressDto.City) ? addressDto.City.Replace(",", "") + "," : string.Empty;
        StateProvince = !string.IsNullOrWhiteSpace(addressDto.StateProvince) ? addressDto.StateProvince.Replace(",", "") + "," : string.Empty;
        PostalCode = addressDto.PostCode;
    }

    private void GetContactDetails()
    {
        //If delivery type is In-Person, get phone from service at location -> link contacts -> contact -> phone
        if (GetDeliveryMethodsAsString(LocalOffer.ServiceDeliveries).Contains("In Person"))
        {
            if (LocalOffer.Locations.Count == 0)
                return;
            var location = LocalOffer.Locations.FirstOrDefault();

            if (location?.Contacts == null || location?.Contacts.Count == 0)
                return;
            var contact = location?.Contacts.FirstOrDefault();
            Phone = contact?.Telephone!;
            Website = contact?.Url!;
            Email = contact?.Email!;
        }
        else
        {
            if (LocalOffer.Contacts == null)
                return;
            //if there are more then one contact then bellow code will pick the last record
            foreach (var contactDto in LocalOffer.Contacts)
            {
                Phone = contactDto.Telephone ?? string.Empty;
                Website = contactDto.Url ?? string.Empty;
                Email = contactDto.Email ?? string.Empty;

                if (string.IsNullOrEmpty(Website))
                    continue;

                if (Website.Length > 4 && string.Compare(Website.Substring(0, 4), "http", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (!IsValidUrl(Website))
                    {
                        Website = string.Empty;
                    }
                    continue;
                }

                Website = $"https://{Website}";

                if (!IsValidUrl(Website))
                {
                    Website = string.Empty;
                }
            }
        }
    }

    bool IsValidUrl(string url)
    {
        var pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
        var rgx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return rgx.IsMatch(url);
    }
}
