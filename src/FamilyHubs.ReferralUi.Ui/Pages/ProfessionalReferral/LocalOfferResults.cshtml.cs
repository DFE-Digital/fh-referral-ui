using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralLanguages;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralPhysicalAddresses;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralServiceDeliverysEx;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralServices;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralTaxonomys;
using FamilyHubs.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class LocalOfferResultsModel : PageModel
{
    private readonly ILocalOfferClientService _localOfferClientService;
    private readonly IPostcodeLocationClientService _postcodeLocationClientService;
    private readonly IOpenReferralOrganisationClientService _openReferralOrganisationClientService;

    public Dictionary<int, string> DictServiceDelivery = new();

    [BindProperty]
    public List<string> ServiceDeliverySelection { get; set; } = default!;

    [BindProperty]
    public List<string> CostSelection { get; set; } = default!;

    public List<KeyValuePair<OpenReferralTaxonomyDto, List<OpenReferralTaxonomyDto>>> NestedCategories { get; set; } = default!;

    public List<OpenReferralTaxonomyDto> Categories { get; set; } = default!;

    [BindProperty]
    public List<string> CategorySelection { get; set; } = default!;
    [BindProperty]
    public List<string> SubcategorySelection { get; set; } = default!;

    public double CurrentLatitude { get; set; }
    public double CurrentLongitude { get; set; }

    public PaginatedList<OpenReferralServiceDto> SearchResults { get; set; } = default!;

    public string SelectedDistance { get; set; } = "212892";

    [BindProperty]
    public bool ForChildrenAndYoungPeople { get; set; } = false;
    [BindProperty]
    public string? SearchAge { get; set; } = default!;
    public List<SelectListItem> AgeRange { get; set; } = default!;

    [BindProperty]
    public string? SelectedLanguage { get; set; } = default!;
    public List<SelectListItem> Languages { get; set; } = default!;

    [BindProperty]
    public bool CanFamilyChooseLocation { get; set; } = false;

    public bool RemoveFilter { get; set; } = false;

    [BindProperty(SupportsGet = true)]
    public string? SearchText { get; set; }

    [BindProperty]
    public string? SearchPostCode { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? OutCode { get; set; }
    public string? DistrictCode { get; set; }

    public string SearchResultsSnippet
    {
        get
        {
            if (SearchResults.TotalCount == 1)
            {
                return $"Showing {SearchResults.TotalCount} search result for:";
            }
            else
            {
                return $"Showing {SearchResults.TotalCount} search results for:";
            }
        }
    }

    public bool InitialLoad { get; set; } = true;
    public List<SelectListItem> DistanceSelectionList { get; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "1609.34", Text = "1 mile" },
        new SelectListItem { Value = "3218.69", Text = "2 miles" },
        new SelectListItem { Value = "8046.72", Text = "5 miles" },
        new SelectListItem { Value = "16093.4", Text = "10 miles" },
        new SelectListItem { Value = "24140.2", Text = "15 miles" },
        new SelectListItem { Value = "32186.9", Text = "20 miles" },
    };

    public LocalOfferResultsModel(ILocalOfferClientService localOfferClientService, IPostcodeLocationClientService postcodeLocationClientService, IOpenReferralOrganisationClientService openReferralOrganisationClientService)
    {
        _localOfferClientService = localOfferClientService;
        _postcodeLocationClientService = postcodeLocationClientService;
        _openReferralOrganisationClientService = openReferralOrganisationClientService;
    }

    public async Task OnGetAsync(string postCode,
                                 double latitude,
                                 double longitude,
                                 double distance,
                                 string minimumAge,
                                 string maximumAge,
                                 string searchText)
    {
        SearchPostCode = postCode;
        await GetLocationDetails(SearchPostCode);
        await GetCategoriesTreeAsync();
        GetCategories();

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
        InitializeLanguages();
        SearchResults = await _localOfferClientService.GetLocalOffers("Information Sharing",
                                                                      "active",
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
                                                                      null,
                                                                      null,
                                                                      null);
    }

    public async Task<IActionResult> OnPostAsync(string? removeCostSelection, bool removeFilter, string? removeServiceDeliverySelection, 
                                                 string? removeSelectedLanguage, string? removeSearchAge, string? removecategorySelection,
                                                 string? removesubcategorySelection)
    {
        if(removeFilter)
        {
            if(removeCostSelection != null) RemoveFilterCostSelection(removeCostSelection);
            if (removeServiceDeliverySelection != null) RemoveFilterServiceDeliverySelection(removeServiceDeliverySelection);
            if (removeSelectedLanguage != null) SelectedLanguage = null;
            if (removeSearchAge != null)
            {
                SearchAge = null;
                ForChildrenAndYoungPeople = false;
            }
                
            if (removecategorySelection != null) RemoveFilterForCategory(removecategorySelection);
            if (removesubcategorySelection != null) RemoveFilterForSubCategory(removesubcategorySelection);


        }
        if (ForChildrenAndYoungPeople && (SearchAge == null || !int.TryParse(SearchAge, out int searchAgeTest)))
        {
            ModelState.AddModelError(nameof(SearchAge), "Please select a valid search age");
        }

        if (!ModelState.IsValid)
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            CreateServiceDeliveryDictionary();
            await GetCategoriesTreeAsync();
            GetCategories();
            InitializeAgeRange();
            InitializeLanguages();
            return Page();
        }

        //Keep these as might be needed at a later stage
        //SelectedDistance = Request.Form["SelectedDistance"];
        SearchText = Request.Form["SearchText"];

        if (SearchPostCode != null)
            await GetLocationDetails(SearchPostCode);
        if (!double.TryParse(SelectedDistance, out double distance))
            distance = 0.0D;
        if (!ForChildrenAndYoungPeople)
            SearchAge = null;
        if (!int.TryParse(SearchAge, out int searchAge))
            searchAge = -1;

        CreateServiceDeliveryDictionary();

        string? serviceDelivery = null;
        if (ServiceDeliverySelection != null && ServiceDeliverySelection.Count>0)
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

        if (SelectedLanguage == "All languages")
            SelectedLanguage = null;


        var taxonomies = string.Join(",", SubcategorySelection);

        SearchResults = await _localOfferClientService.GetLocalOffers("Information Sharing",
                                                                      "active",
                                                                      null,
                                                                      null,
                                                                      (ForChildrenAndYoungPeople && searchAge >= 0) ? searchAge : null,
                                                                      DistrictCode ?? string.Empty,
                                                                      (CurrentLatitude != 0.0D) ? CurrentLatitude : null,
                                                                      (CurrentLongitude != 0.0D) ? CurrentLongitude : null,
                                                                      (distance > 0.0D) ? distance : null,
                                                                      1,
                                                                      99,
                                                                      SearchText ?? string.Empty,
                                                                      serviceDelivery,
                                                                      isPaidFor,
                                                                      taxonomies,
                                                                      SelectedLanguage == "All languages"? null: SelectedLanguage,
                                                                      CanFamilyChooseLocation);

        InitializeAgeRange();
        InitializeLanguages();
        await GetCategoriesTreeAsync();
        GetCategories();
        InitialLoad = false;
        ModelState.Clear();
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
            result = result
                     + (Enum.GetName(serviceDelivery.ServiceDelivery) != null ? Enum.GetName(serviceDelivery.ServiceDelivery) + "," : String.Empty);

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
            new SelectListItem{ Value="-1", Text="All ages" , Selected = true},
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

    private void InitializeLanguages()
    {
        Languages = new List<SelectListItem>() {
        new SelectListItem { Value = "All languages", Text="All languages" , Selected = true},
        new SelectListItem { Value = "Afrikaans", Text = "Afrikaans" },
        new SelectListItem { Value = "Albanian", Text = "Albanian" },
        new SelectListItem { Value = "Arabic", Text = "Arabic" },
        new SelectListItem { Value = "Armenian", Text = "Armenian" },
        new SelectListItem { Value = "Basque", Text = "Basque" },
        new SelectListItem { Value = "Bengali", Text = "Bengali" },
        new SelectListItem { Value = "Bulgarian", Text = "Bulgarian" },
        new SelectListItem { Value = "Catalan", Text = "Catalan" },
        new SelectListItem { Value = "Cambodian", Text = "Cambodian" },
        new SelectListItem { Value = "Chinese (Mandarin)", Text = "Chinese (Mandarin)" },
        new SelectListItem { Value = "Croatian", Text = "Croatian" },
        new SelectListItem { Value = "Czech", Text = "Czech" },
        new SelectListItem { Value = "Danish", Text = "Danish" },
        new SelectListItem { Value = "Dutch", Text = "Dutch" },
        new SelectListItem { Value = "English", Text = "English"},
        new SelectListItem { Value = "Estonian", Text = "Estonian" },
        new SelectListItem { Value = "Fiji", Text = "Fiji" },
        new SelectListItem { Value = "Finnish", Text = "Finnish" },
        new SelectListItem { Value = "French", Text = "French" },
        new SelectListItem { Value = "Georgian", Text = "Georgian" },
        new SelectListItem { Value = "German", Text = "German" },
        new SelectListItem { Value = "Greek", Text = "Greek" },
        new SelectListItem { Value = "Gujarati", Text = "Gujarati" },
        new SelectListItem { Value = "Hebrew", Text = "Hebrew" },
        new SelectListItem { Value = "Hindi", Text = "Hindi" },
        new SelectListItem { Value = "Hungarian", Text = "Hungarian" },
        new SelectListItem { Value = "Icelandic", Text = "Icelandic" },
        new SelectListItem { Value = "Indonesian", Text = "Indonesian" },
        new SelectListItem { Value = "Irish", Text = "Irish" },
        new SelectListItem { Value = "Italian", Text = "Italian" },
        new SelectListItem { Value = "Japanese", Text = "Japanese" },
        new SelectListItem { Value = "Javanese", Text = "Javanese" },
        new SelectListItem { Value = "Korean", Text = "Korean" },
        new SelectListItem { Value = "Latin", Text = "Latin" },
        new SelectListItem { Value = "Latvian", Text = "Latvian" },
        new SelectListItem { Value = "Lithuanian", Text = "Lithuanian" },
        new SelectListItem { Value = "Macedonian", Text = "Macedonian" },
        new SelectListItem { Value = "Malay", Text = "Malay" },
        new SelectListItem { Value = "Malayalam", Text = "Malayalam" },
        new SelectListItem { Value = "Maltese", Text = "Maltese" },
        new SelectListItem { Value = "Maori", Text = "Maori" },
        new SelectListItem { Value = "Marathi", Text = "Marathi" },
        new SelectListItem { Value = "Mongolian", Text = "Mongolian" },
        new SelectListItem { Value = "Nepali", Text = "Nepali" },
        new SelectListItem { Value = "Norwegian", Text = "Norwegian" },
        new SelectListItem { Value = "Persian", Text = "Persian" },
        new SelectListItem { Value = "Polish", Text = "Polish" },
        new SelectListItem { Value = "Portuguese", Text = "Portuguese" },
        new SelectListItem { Value = "Punjabi", Text = "Punjabi" },
        new SelectListItem { Value = "Quechua", Text = "Quechua" },
        new SelectListItem { Value = "Romanian", Text = "Romanian" },
        new SelectListItem { Value = "Russian", Text = "Russian" },
        new SelectListItem { Value = "Samoan", Text = "Samoan" },
        new SelectListItem { Value = "Serbian", Text = "Serbian" },
        new SelectListItem { Value = "Slovak", Text = "Slovak" },
        new SelectListItem { Value = "Slovenian", Text = "Slovenian" },
        new SelectListItem { Value = "Spanish", Text = "Spanish" },
        new SelectListItem { Value = "Swahili", Text = "Swahili" },
        new SelectListItem { Value = "Swedish ", Text = "Swedish " },
        new SelectListItem { Value = "Tamil", Text = "Tamil" },
        new SelectListItem { Value = "Tatar", Text = "Tatar" },
        new SelectListItem { Value = "Telugu", Text = "Telugu" },
        new SelectListItem { Value = "Thai", Text = "Thai" },
        new SelectListItem { Value = "Tibetan", Text = "Tibetan" },
        new SelectListItem { Value = "Tonga", Text = "Tonga" },
        new SelectListItem { Value = "Turkish", Text = "Turkish" },
        new SelectListItem { Value = "Ukrainian", Text = "Ukrainian" },
        new SelectListItem { Value = "Urdu", Text = "Urdu" },
        new SelectListItem { Value = "Uzbek", Text = "Uzbek" },
        new SelectListItem { Value = "Vietnamese", Text = "Vietnamese" },
        new SelectListItem { Value = "Welsh", Text = "Welsh" },
        new SelectListItem { Value = "Xhosa", Text = "Xhosa" },

        };
    }


    public async Task ClearFilters()
    {
        await OnGetAsync(SearchPostCode ?? String.Empty,
                0.0D,
                0.0D,
                0.0D,
                String.Empty,
                String.Empty,
                String.Empty);
    }

    private async Task GetCategoriesTreeAsync()
    {
        List<KeyValuePair<OpenReferralTaxonomyDto, List<OpenReferralTaxonomyDto>>> categories = await _openReferralOrganisationClientService.GetCategories();

        if (categories != null)
            NestedCategories = new List<KeyValuePair<OpenReferralTaxonomyDto, List<OpenReferralTaxonomyDto>>>(categories);

    }

    /// <summary>
    /// Stores category dto's in a list to make it easy to get category name from id
    /// </summary>
    private void GetCategories()
    {
        Categories = new List<OpenReferralTaxonomyDto>();
        foreach (var category in NestedCategories)
        {
            Categories.Add(category.Key);
            foreach (var subcategory in category.Value)
                Categories.Add(subcategory);
        }
    }


    private void RemoveFilterCostSelection(string removeFilter)
    {
        CostSelection.Remove(removeFilter);
        ModelState.Remove(removeFilter);
    }

    private void RemoveFilterServiceDeliverySelection(string removeFilter)
    {
        ServiceDeliverySelection.Remove(removeFilter);
        ModelState.Remove(removeFilter);
    }

    private void RemoveFilterForCategory(string removeFilter)
    {
        CategorySelection.Remove(removeFilter);
        ModelState.Remove(removeFilter);
    }

    private void RemoveFilterForSubCategory(string removeFilter)
    {
        ModelState.Remove(removeFilter);
        SubcategorySelection.Remove(removeFilter);
    }

    
}
