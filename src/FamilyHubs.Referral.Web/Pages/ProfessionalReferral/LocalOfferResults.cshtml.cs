using System.Dynamic;
using EnumsNET;
using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FamilyHubs.ServiceDirectory.Shared.Models;
using FamilyHubs.ServiceDirectory.Shared.ReferenceData;
using FamilyHubs.SharedKernel.Enums;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Identity.Models;
using FamilyHubs.SharedKernel.Razor.Pagination;
using FamilyHubs.SharedKernel.Services.Postcode.Interfaces;
using FamilyHubs.SharedKernel.Services.Postcode.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

[Authorize(Roles = RoleGroups.LaOrVcsProfessionalOrDualRole)]
public class LocalOfferResultsModel : HeaderPageModel
{
    private readonly IPostcodeLookup _postcodeLookup;
    private readonly IOrganisationClientService _organisationClientService;
    private readonly ILogger<LocalOfferResultsModel> _logger;

    public Dictionary<int, string> DictServiceDelivery { get; private set; }
    public List<KeyValuePair<TaxonomyDto, List<TaxonomyDto>>> NestedCategories { get; set; } = default!;
    public List<TaxonomyDto> Categories { get; set; } = default!;
    public double CurrentLatitude { get; set; }
    public double CurrentLongitude { get; set; }
    public PaginatedList<ServiceDto> SearchResults { get; set; } = new();
    public string SelectedDistance { get; set; } = "212892";

    private bool _isInitialSearch = true;

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

    public const string AllLanguagesValue = "all";

    public static SelectListItem[] StaticLanguageOptions { get; set; }
    public IEnumerable<SelectListItem> LanguageOptions => StaticLanguageOptions;

    static LocalOfferResultsModel()
    {
        StaticLanguageOptions = Languages.FilterCodes
            .Select(c => new SelectListItem(Languages.CodeToName[c], c))
            .OrderBy(kv => kv.Text)
            .Prepend(new SelectListItem("All languages", AllLanguagesValue, true))
            .ToArray();
    }

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
    public string Postcode { get; set; } = string.Empty;

    [BindProperty]
    public int PageNum { get; set; } = 1;

    [BindProperty]
    public Guid CorrelationId { get; set; }

    public int PageSize { get; set; } = 10;
    public IPagination Pagination { get; set; }
    public int TotalResults { get; set; }
    public string? DistrictCode { get; set; }

    public bool InitialLoad { get; set; } = true;

    public LocalOfferResultsModel(
        IPostcodeLookup postcodeLookup,
        IOrganisationClientService organisationClientService,
        ILogger<LocalOfferResultsModel> logger)
    {
        DictServiceDelivery = new();
        _postcodeLookup = postcodeLookup;
        _organisationClientService = organisationClientService;
        _logger = logger;

        Pagination = new DontShowPagination();
    }

    public async Task<IActionResult> OnGetAsync(
        string postcode, string? searchText, string? searchAge,
        string? selectedLanguage, string? subcategorySelection,
        string? costSelection, string? serviceDeliverySelection,
        int? pageNum, bool forChildrenAndYoungPeople, Guid? correlationId
        )
    {
        Postcode = postcode;

        if (correlationId is null)
        {
            CorrelationId = Guid.NewGuid();
            // If no correlation ID exists, then treat this search as a
            // new search.
            _isInitialSearch = true;
        }
        else
        {
            CorrelationId = correlationId.Value;
            _isInitialSearch = false;
        }

        SearchText = searchText;
        SearchAge = searchAge;
        SelectedLanguage = selectedLanguage == AllLanguagesValue ? null : selectedLanguage;
        PageNum = pageNum ?? 1;
        ForChildrenAndYoungPeople = forChildrenAndYoungPeople;
        SubcategorySelection = subcategorySelection?.Split(",").ToList();
        CostSelection = costSelection?.Split(",").ToList();
        ServiceDeliverySelection = serviceDeliverySelection?.Split(",").ToList();

        await GetLocationDetails(Postcode);

        //todo: it does this every request!
        await GetCategoriesTreeAsync();

        CreateServiceDeliveryDictionary();

        DateTime requestTimestamp = DateTime.UtcNow;
        HttpResponseMessage? response = await SearchServices();
        DateTime? responseTimestamp = DateTime.UtcNow;

        try
        {
            if (Postcode is not null)
            {
                // If the user is coming from the initial postcode search page,
                // FromPostCodeSearch will be true, and we can use this to differentiate
                // between initial searches, and subsequent search query changes.
                var eventType = _isInitialSearch ? ServiceDirectorySearchEventType.ServiceDirectoryInitialSearch
                    : ServiceDirectorySearchEventType.ServiceDirectorySearchFilter;

                FamilyHubsUser familyHusUser = HttpContext.GetFamilyHubsUser();
                
                await _organisationClientService.RecordServiceSearch(
                    eventType,
                    Postcode!,
                    long.Parse(familyHusUser.AccountId),
                    SearchResults.Items,
                    requestTimestamp,
                    responseTimestamp,
                    response?.StatusCode,
                    CorrelationId
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred storing service search metric. {ExceptionMessage}",
                ex.Message);
        }

        return Page();
    }

    private async Task<HttpResponseMessage?> SearchServices()
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

        bool? allChildrenYoungPeople = null;
        int? givenAge = null;
        if (int.TryParse(SearchAge, out int searchAge))
        {
            if (searchAge == -1)
            {
                allChildrenYoungPeople = ForChildrenAndYoungPeople;
            }
            else
            {
                givenAge = searchAge;
            }
        }
        
        var localOfferFilter = new LocalOfferFilter
        {
            CanFamilyChooseLocation = CanFamilyChooseLocation,
            ServiceType = "InformationSharing",
            Status = "Active",
            PageSize = PageSize,
            IsPaidFor = isPaidFor,
            PageNumber = PageNum,
            Text = SearchText ?? null,
            DistrictCode = DistrictCode ?? null,
            //todo: we need to fix this. can we use null instead? does postcodes.io return exact 0's? do we need to use abs & epsilon?
            Latitude = CurrentLatitude != 0.0D ? CurrentLatitude : null,
            Longitude = CurrentLongitude != 0.0D ? CurrentLongitude : null,
            AllChildrenYoungPeople = allChildrenYoungPeople,
            GivenAge = givenAge,
            Proximity = double.TryParse(SelectedDistance, out var distanceParsed) && distanceParsed > 0.00d ? distanceParsed : null,
            ServiceDeliveries = ServiceDeliverySelection is not null && ServiceDeliverySelection.Any() ? string.Join(',', ServiceDeliverySelection) : null,
            TaxonomyIds = SubcategorySelection is not null && SubcategorySelection.Any() ? string.Join(",", SubcategorySelection) : null,
            LanguageCode = SelectedLanguage != null && SelectedLanguage != AllLanguagesValue ? SelectedLanguage : null
        };

        (SearchResults, HttpResponseMessage? response) = await _organisationClientService.GetLocalOffers(localOfferFilter);
        Pagination = new LargeSetPagination(SearchResults.TotalPages, PageNum);
        TotalResults = SearchResults.TotalCount;

        return response;
    }

    public IActionResult OnPostAsync(
        bool removeFilter,
        string? removeCostSelection, string? removeServiceDeliverySelection,
        string? removeSelectedLanguage, string? removeForChildrenAndYoungPeople,
        string? removeSearchAge, string? removecategorySelection,
        string? removesubcategorySelection)
    {
        var routeValues = ToRouteValuesWithRemovedFilters(
            removeFilter,
            removeCostSelection, removeServiceDeliverySelection,
            removeSelectedLanguage, removeForChildrenAndYoungPeople,
            removeSearchAge, removecategorySelection, removesubcategorySelection);

        InitialLoad = false;
        ModelState.Clear();

        return RedirectToPage("/ProfessionalReferral/LocalOfferResults", routeValues);
    }

    private dynamic ToRouteValuesWithRemovedFilters(bool removeFilter,
        string? removeCostSelection, string? removeServiceDeliverySelection,
        string? removeSelectedLanguage, string? removeForChildrenAndYoungPeople,
        string? removeSearchAge, string? removecategorySelection,
        string? removesubcategorySelection)
    {
        dynamic routeValues = new ExpandoObject();
        var routeValuesDictionary = (IDictionary<string, object>)routeValues;

        foreach (var keyValuePair in Request.Form.Where(f => !f.Key.Contains("__") && !f.Key.Contains("remove")))
        {
            if (removeFilter)
            {
                if (removeSelectedLanguage != null && keyValuePair.Key is nameof(SelectedLanguage))
                {
                    continue;
                }

                if (removeForChildrenAndYoungPeople != null && keyValuePair.Key is nameof(ForChildrenAndYoungPeople))
                {
                    continue;
                }

                if ((removeSearchAge != null || removeForChildrenAndYoungPeople != null)
                    && keyValuePair.Key is nameof(SearchAge))
                {
                    continue;
                }

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
        DictServiceDelivery = Enum.GetValues(typeof(AttendingType))
            .Cast<AttendingType>()
            .Where(d => (int)d != 0)
            .ToDictionary(k => (int)k, v => v.ToDescription());
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
        var (postcodeError, postcodeInfo) = await _postcodeLookup.Get(postCode);

        //todo: we shouldn't ignore the error, but this is what it's always done
        //todo: what we should really do is pass this info on from the postcode search page
        if (postcodeError == PostcodeError.None)
        {
            CurrentLatitude = postcodeInfo!.Latitude!.Value;
            CurrentLongitude = postcodeInfo.Longitude!.Value;
            DistrictCode = postcodeInfo.AdminArea;
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
