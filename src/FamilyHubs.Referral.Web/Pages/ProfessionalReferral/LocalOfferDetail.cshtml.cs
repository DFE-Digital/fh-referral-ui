using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.ServiceDirectory.Shared.Enums;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

//todo: this is going to be moved to service directory common for reuse
public static class ServiceDisplayExtensions
{
    public static IEnumerable<string> GetServiceAvailability(this ICollection<ScheduleDto>? schedules)
    {
        if (schedules == null || schedules.Count == 0)
            return Enumerable.Empty<string>();

        var weekdaysAndWeekends = GetWeekdaysAndWeekends(schedules).ToArray();
        var timeDescription = GetTimeDescription(schedules);

        bool hasWeekdaysAndWeekends = weekdaysAndWeekends.Length > 0;

        if (!hasWeekdaysAndWeekends && timeDescription == null)
            return Enumerable.Empty<string>();

        if (hasWeekdaysAndWeekends && timeDescription != null)
        {
            return weekdaysAndWeekends.Append("").Append(timeDescription);
        }

        return hasWeekdaysAndWeekends ? weekdaysAndWeekends : new[] { timeDescription! };
    }

    private static string? GetTimeDescription(this ICollection<ScheduleDto> schedules)
    {
        return schedules.FirstOrDefault(x => x.Description != null)?.Description;
    }

    private static IEnumerable<string> GetWeekdaysAndWeekends(this ICollection<ScheduleDto> schedules)
    {
        return schedules
            .Select(ScheduleDescription)
            .Where(d => !string.IsNullOrEmpty(d))!;
    }

    private static string? ScheduleDescription(ScheduleDto schedule)
    {
        if (schedule.Freq != FrequencyType.Weekly)
        {
            return null;
        }

        string dayType;
        switch (schedule.ByDay)
        {
            case "MO,TU,WE,TH,FR":
                dayType = "Weekdays";
                break;
            case "SA,SU":
                dayType = "Weekends";
                break;
            default:
                return null;
        }

        return $"{dayType}: {schedule.OpensAt:h:mmtt} to {schedule.ClosesAt:h:mmtt}";
    }
}

[Authorize(Roles = RoleGroups.LaOrVcsProfessionalOrDualRole)]
public class LocalOfferDetailModel : HeaderPageModel
{
    private readonly IOrganisationClientService _organisationClientService;
    private readonly IIdamsClient _idamsClient;
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

    public LocalOfferDetailModel(
        IOrganisationClientService organisationClientService,
        IIdamsClient idamsClient)
    {
        _organisationClientService = organisationClientService;
        _idamsClient = idamsClient;
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

        ShowConnectionRequestButton = await ShouldShowConnectionRequestButton();

        return Page();
    }

    private async Task<bool> ShouldShowConnectionRequestButton()
    {
        bool showConnectionRequestButton = HttpContext.GetRole() is
            RoleTypes.LaProfessional or RoleTypes.LaDualRole;
        if (showConnectionRequestButton)
        {
            var vcsProEmails = await _idamsClient
                .GetVcsProfessionalsEmailsAsync(LocalOffer.OrganisationId);
            showConnectionRequestButton = vcsProEmails.Any();
        }

        return showConnectionRequestButton;
    }

    public string GetDeliveryMethodsAsString(ICollection<ServiceDeliveryDto>? serviceDeliveries)
    {
        var result = string.Empty;

        if (serviceDeliveries == null || serviceDeliveries.Count == 0)
            return result;

        result = string.Join(',', serviceDeliveries.Select(serviceDelivery => serviceDelivery.Name).ToArray());

        return result;
    }

    //todo: don't show languages if there are none
    public string GetLanguagesAsString(ICollection<LanguageDto>? languageDtos)
    {
        // they should already be sorted by name, but for safety we do it again
        return string.Join(", ", languageDtos?.Select(d => d.Name).Order() ?? Enumerable.Empty<string>());
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

            if (location?.Contacts == null || location.Contacts.Count == 0)
                return;
            var contact = location.Contacts.First();
            Phone = contact.Telephone;
            Website = contact.Url!;
            Email = contact.Email!;
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
