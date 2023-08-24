using System.Dynamic;
using System.Text;
using EnumsNET;
using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FamilyHubs.ServiceDirectory.Shared.Models;
using FamilyHubs.SharedKernel.Razor.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

[Authorize]
public class LocalOfferResultsModel : PageModel
{
    private readonly IPostcodeLocationClientService _postcodeLocationClientService;
    private readonly IOrganisationClientService _organisationClientService;

    public Dictionary<int, string> DictServiceDelivery { get; private set; }
    public List<KeyValuePair<TaxonomyDto, List<TaxonomyDto>>> NestedCategories { get; set; } = default!;
    public List<TaxonomyDto> Categories { get; set; } = default!;
    public double CurrentLatitude { get; set; }
    public double CurrentLongitude { get; set; }
    public PaginatedList<ServiceDto> SearchResults { get; set; } = new();
    public string SelectedDistance { get; set; } = "212892";
    public List<SelectListItem> AgeRange { get; set; } = new()
    {
        new() { Value="-1", Text="All ages" , Selected = true},
        new() { Value="0", Text="0 to 12 months" },
        new() { Value="1", Text="1 year old"},
        new() { Value="2", Text="2 years old"},
        new() { Value="3", Text="3 years old"},
        new() { Value="4", Text="4 years old"},
        new() { Value="5", Text="5 years old"},
        new() { Value="6", Text="6 years old"},
        new() { Value="7", Text="7 years old"},
        new() { Value="8", Text="8 years old"},
        new() { Value="9", Text="9 years old"},
        new() { Value="10", Text="10 years old"},
        new() { Value="11", Text="11 years old"},
        new() { Value="12", Text="12 years old"},
        new() { Value="13", Text="13 years old"},
        new() { Value="14", Text="14 years old"},
        new() { Value="15", Text="15 years old"},
        new() { Value="16", Text="16 years old"},
        new() { Value="17", Text="17 years old"},
        new() { Value="18", Text="18 years old"},
        new() { Value="19", Text="19 years old"},
        new() { Value="20", Text="20 years old"},
        new() { Value="21", Text="21 years old"},
        new() { Value="22", Text="22 years old"},
        new() { Value="23", Text="23 years old"},
        new() { Value="24", Text="24 years old"},
        new() { Value="25", Text="25 years old"},
    };
    public List<SelectListItem> Languages { get; set; } = new()
    {
        new() { Value = "All languages", Text="All languages" , Selected = true},
        new() { Value = "Afrikaans", Text = "Afrikaans" },
        new() { Value = "Albanian", Text = "Albanian" },
        new() { Value = "Arabic", Text = "Arabic" },
        new() { Value = "Armenian", Text = "Armenian" },
        new() { Value = "Basque", Text = "Basque" },
        new() { Value = "Bengali", Text = "Bengali" },
        new() { Value = "Bulgarian", Text = "Bulgarian" },
        new() { Value = "Catalan", Text = "Catalan" },
        new() { Value = "Cambodian", Text = "Cambodian" },
        new() { Value = "Chinese (Mandarin)", Text = "Chinese (Mandarin)" },
        new() { Value = "Croatian", Text = "Croatian" },
        new() { Value = "Czech", Text = "Czech" },
        new() { Value = "Danish", Text = "Danish" },
        new() { Value = "Dutch", Text = "Dutch" },
        new() { Value = "English", Text = "English"},
        new() { Value = "Estonian", Text = "Estonian" },
        new() { Value = "Fiji", Text = "Fiji" },
        new() { Value = "Finnish", Text = "Finnish" },
        new() { Value = "French", Text = "French" },
        new() { Value = "Georgian", Text = "Georgian" },
        new() { Value = "German", Text = "German" },
        new() { Value = "Greek", Text = "Greek" },
        new() { Value = "Gujarati", Text = "Gujarati" },
        new() { Value = "Hebrew", Text = "Hebrew" },
        new() { Value = "Hindi", Text = "Hindi" },
        new() { Value = "Hungarian", Text = "Hungarian" },
        new() { Value = "Icelandic", Text = "Icelandic" },
        new() { Value = "Indonesian", Text = "Indonesian" },
        new() { Value = "Irish", Text = "Irish" },
        new() { Value = "Italian", Text = "Italian" },
        new() { Value = "Japanese", Text = "Japanese" },
        new() { Value = "Javanese", Text = "Javanese" },
        new() { Value = "Korean", Text = "Korean" },
        new() { Value = "Latin", Text = "Latin" },
        new() { Value = "Latvian", Text = "Latvian" },
        new() { Value = "Lithuanian", Text = "Lithuanian" },
        new() { Value = "Macedonian", Text = "Macedonian" },
        new() { Value = "Malay", Text = "Malay" },
        new() { Value = "Malayalam", Text = "Malayalam" },
        new() { Value = "Maltese", Text = "Maltese" },
        new() { Value = "Maori", Text = "Maori" },
        new() { Value = "Marathi", Text = "Marathi" },
        new() { Value = "Mongolian", Text = "Mongolian" },
        new() { Value = "Nepali", Text = "Nepali" },
        new() { Value = "Norwegian", Text = "Norwegian" },
        new() { Value = "Persian", Text = "Persian" },
        new() { Value = "Polish", Text = "Polish" },
        new() { Value = "Portuguese", Text = "Portuguese" },
        new() { Value = "Punjabi", Text = "Punjabi" },
        new() { Value = "Quechua", Text = "Quechua" },
        new() { Value = "Romanian", Text = "Romanian" },
        new() { Value = "Russian", Text = "Russian" },
        new() { Value = "Samoan", Text = "Samoan" },
        new() { Value = "Serbian", Text = "Serbian" },
        new() { Value = "Slovak", Text = "Slovak" },
        new() { Value = "Slovenian", Text = "Slovenian" },
        new() { Value = "Somali", Text = "Somali" },
        new() { Value = "Spanish", Text = "Spanish" },
        new() { Value = "Swahili", Text = "Swahili" },
        new() { Value = "Swedish ", Text = "Swedish " },
        new() { Value = "Tamil", Text = "Tamil" },
        new() { Value = "Tatar", Text = "Tatar" },
        new() { Value = "Telugu", Text = "Telugu" },
        new() { Value = "Thai", Text = "Thai" },
        new() { Value = "Tibetan", Text = "Tibetan" },
        new() { Value = "Tonga", Text = "Tonga" },
        new() { Value = "Turkish", Text = "Turkish" },
        new() { Value = "Ukrainian", Text = "Ukrainian" },
        new() { Value = "Urdu", Text = "Urdu" },
        new() { Value = "Uzbek", Text = "Uzbek" },
        new() { Value = "Vietnamese", Text = "Vietnamese" },
        new() { Value = "Welsh", Text = "Welsh" },
        new() { Value = "Xhosa", Text = "Xhosa" },
    };

    [BindProperty]
    public List<string>? ServiceDeliverySelection { get; set; }

    [BindProperty]
    public List<string>? CostSelection { get; set; }

    [BindProperty]
    public List<string>? CategorySelection { get; set; }

    [BindProperty]
    public List<string>? SubcategorySelection { get; set; }

    [BindProperty]
    public bool ForChildrenAndYoungPeople { get; set; }

    [BindProperty]
    public string? SearchAge { get; set; }

    [BindProperty]
    public string? SelectedLanguage { get; set; }

    [BindProperty]
    public bool CanFamilyChooseLocation { get; set; } = false;

    [BindProperty]
    public string? SearchText { get; set; }

    [BindProperty]
    public string postcode { get; set; } = string.Empty;

    [BindProperty]
    public int PageNum { get; set; } = 1;

    public int PageSize { get; set; } = 10;
    public IPagination Pagination { get; set; }
    public int TotalResults { get; set; }
    public string? OutCode { get; set; }
    public string? DistrictCode { get; set; }

    public bool InitialLoad { get; set; } = true;

    public LocalOfferResultsModel(IPostcodeLocationClientService postcodeLocationClientService, IOrganisationClientService organisationClientService)
    {
        DictServiceDelivery = new();
        _postcodeLocationClientService = postcodeLocationClientService;
        _organisationClientService = organisationClientService;
        Pagination = new DontShowPagination();
    }

    public async Task<IActionResult> OnGetAsync(
        string postcode, string? searchText, string? searchAge,
        string? selectedLanguage, string? subcategorySelection,
        string? costSelection, string? serviceDeliverySelection,
        int? pageNum, bool forChildrenAndYoungPeople
        )
    {
        this.postcode = postcode;
        SearchText = searchText;
        SearchAge = searchAge;
        SelectedLanguage = selectedLanguage == "All languages" ? null : selectedLanguage;
        PageNum = pageNum ?? 1;
        ForChildrenAndYoungPeople = forChildrenAndYoungPeople;
        SubcategorySelection = subcategorySelection?.Split(",").ToList();
        CostSelection = costSelection?.Split(",").ToList();
        ServiceDeliverySelection = serviceDeliverySelection?.Split(",").ToList();

        await GetLocationDetails(this.postcode);

        await GetCategoriesTreeAsync();

        CreateServiceDeliveryDictionary();

        await SearchServices();

        return Page();
    }

    private async Task SearchServices()
    {
        bool? isPaidFor = null;

        if (CostSelection is not null && CostSelection.Count == 1)
        {
            isPaidFor = CostSelection[0] switch
            {
                "paid" => true,
                "free" => false,
                _ => null
            };
        }

        var localOfferFilter = new LocalOfferFilter
        {
            CanFamilyChooseLocation = CanFamilyChooseLocation,
            ServiceType = "InformationSharing",
            Status = "Active",
            MinimumAge = null,
            MaximumAge = null,
            PageSize = PageSize,
            IsPaidFor = isPaidFor,
            PageNumber = PageNum,
            Text = SearchText ?? null,
            DistrictCode = DistrictCode ?? null,
            Latitude = CurrentLatitude != 0.0D ? CurrentLatitude : null,
            Longitude = CurrentLongitude != 0.0D ? CurrentLongitude : null,
            GivenAge = ForChildrenAndYoungPeople && int.TryParse(SearchAge, out var searchAgeResult) ? searchAgeResult : null,
            Proximity = double.TryParse(SelectedDistance, out var distanceParsed) && distanceParsed > 0.00d ? distanceParsed : null,
            ServiceDeliveries = ServiceDeliverySelection is not null && ServiceDeliverySelection.Any() ? string.Join(',', ServiceDeliverySelection) : null,
            TaxonomyIds = SubcategorySelection is not null && SubcategorySelection.Any() ? string.Join(",", SubcategorySelection) : null,
            Languages = SelectedLanguage is not null && SelectedLanguage is not "All languages" ? SelectedLanguage : null
        };

        SearchResults = await _organisationClientService.GetLocalOffers(localOfferFilter);

        Pagination = new LargeSetPagination(SearchResults.TotalPages, PageNum);

        TotalResults = SearchResults.TotalCount;
    }

    public IActionResult OnPostAsync(
        bool removeFilter,
        string? removeCostSelection, string? removeServiceDeliverySelection,
        string? removeSelectedLanguage, string? removeSearchAge,
        string? removecategorySelection, string? removesubcategorySelection)
    {
        var routeValues = ToRouteValuesWithRemovedFilters(
            removeFilter,
            removeCostSelection, removeServiceDeliverySelection,
            removeSelectedLanguage, removeSearchAge,
            removecategorySelection, removesubcategorySelection);

        InitialLoad = false;
        ModelState.Clear();

        return RedirectToPage("/ProfessionalReferral/LocalOfferResults", routeValues);
    }

    private dynamic ToRouteValuesWithRemovedFilters(bool removeFilter,
        string? removeCostSelection, string? removeServiceDeliverySelection,
        string? removeSelectedLanguage, string? removeSearchAge,
        string? removecategorySelection, string? removesubcategorySelection)
    {
        dynamic routeValues = new ExpandoObject();
        var routeValuesDictionary = (IDictionary<string, object>)routeValues;

        foreach (var keyValuePair in Request.Form.Where(f => !f.Key.Contains("__") && !f.Key.Contains("remove")))
        {
            if (removeFilter)
            {
                if (removeSelectedLanguage != null && keyValuePair.Key is nameof(SelectedLanguage)) continue;
                if (removeSearchAge != null && keyValuePair.Key is nameof(SearchAge) or nameof(ForChildrenAndYoungPeople)) continue;

                if (removeCostSelection != null && keyValuePair.Key is nameof(CostSelection))
                {
                    routeValuesDictionary[keyValuePair.Key] = string.Join(",", keyValuePair.Value.ToString()
                        .Split(",").Where(s => s != removeCostSelection));
                    continue;
                }

                if (removeServiceDeliverySelection != null && keyValuePair.Key is nameof(ServiceDeliverySelection))
                {
                    routeValuesDictionary[keyValuePair.Key] = string.Join(",", keyValuePair.Value.ToString()
                        .Split(",").Where(s => s != removeServiceDeliverySelection));
                    continue;
                }

                if (removecategorySelection != null && keyValuePair.Key is nameof(CategorySelection))
                {
                    routeValuesDictionary[keyValuePair.Key] = string.Join(",", keyValuePair.Value.ToString()
                        .Split(",").Where(s => s != removecategorySelection));
                    continue;
                }

                if (removesubcategorySelection != null && keyValuePair.Key is nameof(SubcategorySelection))
                {
                    routeValuesDictionary[keyValuePair.Key] = string.Join(",", keyValuePair.Value.ToString()
                        .Split(",").Where(s => s != removesubcategorySelection));
                    continue;
                }
            }

            routeValuesDictionary[keyValuePair.Key] = keyValuePair.Value.ToString();
        }

        return routeValues;
    }

    private void CreateServiceDeliveryDictionary()
    {
        DictServiceDelivery = Enum.GetValues(typeof(ServiceDeliveryType))
            .Cast<ServiceDeliveryType>()
            .Where(d => (int)d != 0)
            .ToDictionary(k => (int)k, v => Utility.GetEnumDescription(v));
    }

    public string GetAddressAsString(LocationDto? addressDto)
    {
        if (addressDto == null || addressDto.Address1 == string.Empty) return string.Empty;

        var result = new StringBuilder();

        result.Append(addressDto.Address1.Replace("|", ",") + ",");
        result.Append(!string.IsNullOrWhiteSpace(addressDto.City) ? addressDto.City + "," : string.Empty);
        result.Append(!string.IsNullOrWhiteSpace(addressDto.StateProvince) ? addressDto.StateProvince + "," : string.Empty);
        result.Append(addressDto.PostCode);

        return result.ToString();
    }

    public string GetDeliveryMethodsAsString(ICollection<ServiceDeliveryDto> serviceDeliveries)
    {
        return serviceDeliveries.Count == 0
            ? string.Empty
            : string.Join(",", serviceDeliveries.Select(serviceDelivery => serviceDelivery.Name.AsString(EnumFormat.Description)));
    }

    public string GetLanguagesAsString(ICollection<LanguageDto> languageDtos)
    {
        return languageDtos.Count == 0
            ? string.Empty
            : string.Join(",", languageDtos.Select(serviceDelivery => serviceDelivery.Name));
    }

    private async Task GetLocationDetails(string postCode)
    {
        try
        {
            var postcodesIoResponse = await _postcodeLocationClientService.LookupPostcode(postCode);
            CurrentLatitude = postcodesIoResponse.Result.Latitude;
            CurrentLongitude = postcodesIoResponse.Result.Longitude;
            DistrictCode = postcodesIoResponse.Result.AdminArea;
            OutCode = postcodesIoResponse.Result.OutCode;
        }
        catch
        {
            //If post code is not valid then just return
        }
    }

    private async Task GetCategoriesTreeAsync()
    {
        var categories = await _organisationClientService.GetCategories();
        NestedCategories = new(categories);

        Categories = new();

        foreach (var category in NestedCategories)
        {
            Categories.Add(category.Key);
            foreach (var subcategory in category.Value)
                Categories.Add(subcategory);
        }
    }
}