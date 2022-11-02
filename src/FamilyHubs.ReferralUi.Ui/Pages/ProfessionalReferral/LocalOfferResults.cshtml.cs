using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralLanguages;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralPhysicalAddresses;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralServiceDeliverysEx;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralServices;
using FamilyHubs.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class LocalOfferResultsModel : PageModel
{
    private readonly ILocalOfferClientService _localOfferClientService;
    private readonly IPostcodeLocationClientService _postcodeLocationClientService;

    public Dictionary<int, string> DictServiceDelivery = new();

    [BindProperty]
    public List<string> ServiceDeliverySelection { get; set; } = default!;

    [BindProperty]
    public List<string> CostSelection { get; set; } = default!;

    public double CurrentLatitude { get; set; }
    public double CurrentLongitude { get; set; }

    public PaginatedList<OpenReferralServiceDto> SearchResults { get; set; } = default!;

    public string SelectedDistance { get; set; } = "212892";

    [BindProperty]
    public bool ForChildrenAndYoungPeople { get; set; } = false;
    [BindProperty]
    public string SearchAge { get; set; } = default!;
    public List<SelectListItem> AgeRange { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string MinimumAge { get; set; } = "0";
    [BindProperty(SupportsGet = true)]
    public string MaximumAge { get; set; } = "99";
    [BindProperty(SupportsGet = true)]
    public string? SearchText { get; set; }

    [BindProperty]
    public string? SearchPostCode { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? OutCode { get; set; }
    public string? DistrictCode { get; set; }

    public List<SelectListItem> DistanceSelectionList { get; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "1609.34", Text = "1 mile" },
        new SelectListItem { Value = "3218.69", Text = "2 miles" },
        new SelectListItem { Value = "8046.72", Text = "5 miles" },
        new SelectListItem { Value = "16093.4", Text = "10 miles" },
        new SelectListItem { Value = "24140.2", Text = "15 miles" },
        new SelectListItem { Value = "32186.9", Text = "20 miles" },
    };

    public LocalOfferResultsModel(ILocalOfferClientService localOfferClientService, IPostcodeLocationClientService postcodeLocationClientService)
    {
        _localOfferClientService = localOfferClientService;
        _postcodeLocationClientService = postcodeLocationClientService;
    }

    public async Task OnGetAsync(string postCode, double latitude, double longitude, double distance, string minimumAge, string maximumAge, string searchText)
    {
        SearchPostCode = postCode;
        await GetLocationDetails(SearchPostCode);

        //Keep these as might be needed at a later stage
        SelectedDistance = distance.ToString();
        if (searchText != null)
            SearchText = searchText;

        if (!int.TryParse(minimumAge, out int minAge))
            minAge = 0;

        if (!int.TryParse(maximumAge, out int maxAge))
            maxAge = 99;

        CreateServiceDeliveryDictionary();
        InitializeAgeRange();
        SearchResults = await _localOfferClientService.GetLocalOffers("active",
                                                                      null,
                                                                      null,
                                                                      null,
                                                                      DistrictCode ?? string.Empty,
                                                                      (CurrentLatitude != 0.0D) ? CurrentLatitude : null,
                                                                      (CurrentLongitude != 0.0D) ? CurrentLongitude : null,
                                                                      (distance > 0.0D) ? distance : null,
                                                                      CurrentPage,
                                                                      PageSize,
                                                                      SearchText ?? string.Empty,
                                                                      null,
                                                                      null,
                                                                      null);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        //Keep these as might be needed at a later stage
        SelectedDistance = Request.Form["SelectedDistance"];
        SearchText = Request.Form["SearchText"];

        if (SearchPostCode != null)
            await GetLocationDetails(SearchPostCode);

        if (!double.TryParse(SelectedDistance, out double distance))
        {
            distance = 0.0D;
        }

        if (!int.TryParse(MinimumAge, out int minimumAge))
        {
            minimumAge = 0;
        }

        if (!int.TryParse(MaximumAge, out int maximumAge))
        {
            maximumAge = 99;
        }

        if (!int.TryParse(SearchAge, out int searchAge))
            searchAge = 99;

        CreateServiceDeliveryDictionary();
        InitializeAgeRange();

        string? serviceDelivery = null;
        if (ServiceDeliverySelection != null)
            serviceDelivery = string.Join(',', ServiceDeliverySelection.ToArray());

        bool? isPaidFor = null;
        if (CostSelection != null && CostSelection.Count() == 1)
        {
            switch (CostSelection[0])
            {
                case "paid":
                    isPaidFor = true;
                    break;

                case "free":
                    isPaidFor = false;
                    break;
            }
        }

        SearchResults = await _localOfferClientService.GetLocalOffers("active", null, null, (ForChildrenAndYoungPeople && searchAge > 0) ? searchAge : null, DistrictCode ?? string.Empty, (CurrentLatitude != 0.0D) ? CurrentLatitude : null, (CurrentLongitude != 0.0D) ? CurrentLongitude : null, (distance > 0.0D) ? distance : null, 1, 99, SearchText ?? string.Empty, serviceDelivery, isPaidFor, null);

        return Page();

    }

    private void CreateServiceDeliveryDictionary()
    {
        var myEnumDescriptions = from ServiceDelivery n in Enum.GetValues(typeof(ServiceDelivery))
                                 select new { Id = (int)n, Name = Utility.GetEnumDescription(n) };

        foreach (var myEnumDescription in myEnumDescriptions)
        {
            if (myEnumDescription.Id == 0)
                continue;
            DictServiceDelivery[myEnumDescription.Id] = myEnumDescription.Name;
        }
    }

    public string GetAddressAsString(OpenReferralPhysicalAddressDto addressDto)
    {
        string result = string.Empty;

        if (addressDto.Address_1 == null || addressDto.Address_1 == string.Empty)
        {
            return result;
        }

        result = result + (addressDto.Address_1 != null ? addressDto.Address_1 + "," : string.Empty);
        result = result + (addressDto.City != null ? addressDto.City + "," : string.Empty);
        result = result + (addressDto.State_province != null ? addressDto.State_province + "," : string.Empty);
        result = result + (addressDto.Postal_code != null ? addressDto.Postal_code : string.Empty);

        return result;
    }

    public string GetDeliveryMethodsAsString(ICollection<OpenReferralServiceDeliveryExDto> serviceDeliveries)
    {
        string result = string.Empty;

        if (serviceDeliveries == null || serviceDeliveries.Count == 0)
            return result;

        foreach (var serviceDelivery in serviceDeliveries)
            result = result + (Enum.GetName(serviceDelivery.ServiceDelivery) != null ? Enum.GetName(serviceDelivery.ServiceDelivery) + "," : String.Empty);

        //Remove last comma if present
        if (result.EndsWith(","))
        {
            result = result.Remove(result.Length - 1);
        }

        return result;
    }

    public string GetLanguagesAsString(ICollection<OpenReferralLanguageDto> languageDtos)
    {
        string result = string.Empty;

        if (languageDtos == null || languageDtos.Count == 0)
            return result;

        foreach (var language in languageDtos)
            result = result + (language.Language != null ? language.Language + "," : String.Empty);

        //Remove last comma if present
        if (result.EndsWith(","))
        {
            result = result.Remove(result.Length - 1);
        }

        return result;
    }

    private async Task GetLocationDetails(string postCode)
    {
        if (postCode == null)
            return;

        try
        {
            PostcodeApiModel postcodeApiModel = await _postcodeLocationClientService.LookupPostcode(postCode);
            if (postcodeApiModel != null)
            {
                CurrentLatitude = postcodeApiModel.result.latitude;
                CurrentLongitude = postcodeApiModel.result.longitude;
                DistrictCode = postcodeApiModel.result.codes.admin_district;
                OutCode = postcodeApiModel.result.outcode;
            }
        }
        catch
        {
            return;
        }
    }

    private void InitializeAgeRange()
    {
        AgeRange = new List<SelectListItem>() {
            new SelectListItem{ Value="-1", Text="All ages" },
            new SelectListItem{ Value="0", Text="0 to 12 months" },
            new SelectListItem{ Value="1", Text="1 year old"},
            new SelectListItem{ Value="2", Text="2 years old"},
            new SelectListItem{ Value="3", Text="3 years old"},
            new SelectListItem{ Value="4", Text="4 years old"},
            new SelectListItem{ Value="5", Text="5 years old"},
            new SelectListItem{ Value="6", Text="6 years old"},
            new SelectListItem{ Value="7", Text="7 years old"},
            new SelectListItem{ Value="8", Text="8 years old"},
            new SelectListItem{ Value="9", Text="9 years old"},
            new SelectListItem{ Value="10", Text="10 years old"},
            new SelectListItem{ Value="11", Text="11 years old"},
            new SelectListItem{ Value="12", Text="12 years old"},
            new SelectListItem{ Value="13", Text="13 years old"},
            new SelectListItem{ Value="14", Text="14 years old"},
            new SelectListItem{ Value="15", Text="15 years old"},
            new SelectListItem{ Value="16", Text="16 years old"},
            new SelectListItem{ Value="17", Text="17 years old"},
            new SelectListItem{ Value="18", Text="18 years old"},
            new SelectListItem{ Value="19", Text="19 years old"},
            new SelectListItem{ Value="20", Text="20 years old"},
            new SelectListItem{ Value="21", Text="21 years old"},
            new SelectListItem{ Value="22", Text="22 years old"},
            new SelectListItem{ Value="23", Text="23 years old"},
            new SelectListItem{ Value="24", Text="24 years old"},
            new SelectListItem{ Value="25", Text="25 years old"},

        };
    }
}
