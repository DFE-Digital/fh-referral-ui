using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using FamilyHubs.ServiceDirectory.Shared.Dto;
using FamilyHubs.ServiceDirectory.Shared.Enums;
using FamilyHubs.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Net.Mime.MediaTypeNames;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public class LocalOfferResultsOldModel : PageModel
{
    private readonly ILocalOfferClientService _localOfferClientService;
    private readonly IPostcodeLocationClientService _postcodeLocationClientService;

    public Dictionary<int, string> DictServiceDelivery { get; private set; } = new();

    [BindProperty]
    public List<string> ServiceDeliverySelection { get; set; } = default!;

    [BindProperty]
    public List<string> CostSelection { get; set; } = default!;

    public double CurrentLatitude { get; set; }
    public double CurrentLongitude { get; set; }

    public PaginatedList<ServiceDto> SearchResults { get; set; } = default!;

    public string SelectedDistance { get; set; } = "212892";

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

    public LocalOfferResultsOldModel(ILocalOfferClientService localOfferClientService, IPostcodeLocationClientService postcodeLocationClientService)
    {
        _localOfferClientService = localOfferClientService;
        _postcodeLocationClientService = postcodeLocationClientService;
    }

    public async Task OnGetAsync(double latitude, double longitude, double distance, string minimumAge, string maximumAge, string searchText)
    {
        CurrentLatitude = latitude;
        CurrentLongitude = longitude;
        SelectedDistance = distance.ToString();
        if (!int.TryParse(minimumAge, out int minAge))
        {
            minAge = 0;
        }

        if (!int.TryParse(maximumAge, out int maxAge))
        {
            maxAge = 99;
        }

        if (searchText != null)
        {
            SearchText = searchText;
        }

        CreateServiceDeliveryDictionary();

        LocalOfferFilter localOfferFilter = new()
        {
            ServiceType = "Information Sharing",
            Status = "active",
            MinimumAge = minAge,
            MaximumAge = maxAge,
            GivenAge = null,
            DistrictCode = DistrictCode ?? string.Empty,
            Latitude = (latitude != 0.0D) ? latitude : null,
            Longtitude = (longitude != 0.0D) ? longitude : null,
            Proximity = (distance > 0.0D) ? distance : null,
            PageNumber = CurrentPage,
            PageSize = PageSize,
            Text = SearchText ?? string.Empty,
            ServiceDeliveries = null,
            IsPaidFor = null,
            TaxonmyIds = null,
            Languages = null,
            CanFamilyChooseLocation = null
        };

        SearchResults = await _localOfferClientService.GetLocalOffers(localOfferFilter);

    }

    public async Task<IActionResult> OnPost()
    {
        if (double.TryParse(Request.Form["CurrentLatitude"], out double currentLatitude))
        {
            CurrentLatitude = currentLatitude;
        }
        if (double.TryParse(Request.Form["CurrentLongitude"], out double currentLongitude))
        {
            CurrentLongitude = currentLongitude;
        }

        if (SearchPostCode != null)
        {
            await GetPostCode();
        }

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

        CreateServiceDeliveryDictionary();

        string? serviceDelivery = null;
        if (ServiceDeliverySelection != null)
        {
            serviceDelivery = string.Join(',', ServiceDeliverySelection.ToArray());
        }

        bool? isPaidFor = null;
        if (CostSelection != null && CostSelection.Count == 1)
        {
            switch(CostSelection[0])
            {
                case "paid":
                    isPaidFor = true;
                    break;

                case "free":
                    isPaidFor = false;
                    break;
            }
        }

        LocalOfferFilter localOfferFilter = new()
        {
            ServiceType = "Information Sharing",
            Status = "active",
            MinimumAge = minimumAge,
            MaximumAge = maximumAge,
            GivenAge = null,
            DistrictCode = DistrictCode ?? string.Empty,
            Latitude = (CurrentLatitude != 0.0D) ? CurrentLatitude : null,
            Longtitude = (CurrentLongitude != 0.0D) ? CurrentLongitude : null,
            Proximity = (distance > 0.0D) ? distance : null,
            PageNumber = 1,
            PageSize = 99,
            Text = SearchText ?? string.Empty,
            ServiceDeliveries = serviceDelivery,
            IsPaidFor = isPaidFor,
            TaxonmyIds = null,
            Languages = null,
            CanFamilyChooseLocation = null
        };

        SearchResults = await _localOfferClientService.GetLocalOffers(localOfferFilter);

        return Page();
        
    }

    private void CreateServiceDeliveryDictionary()
    {
        var myEnumDescriptions = from ServiceDeliveryType n in Enum.GetValues(typeof(ServiceDeliveryType))
                                 select new { Id = (int)n, Name = Utility.GetEnumDescription(n) };

        foreach (var myEnumDescription in myEnumDescriptions)
        {
            if (myEnumDescription.Id == 0)
                continue;
            DictServiceDelivery[myEnumDescription.Id] = myEnumDescription.Name;
        }
    }

    private async Task GetPostCode()
    {
        if (SearchPostCode == null)
            return;

        try
        {
            var postcodesIoResponse = await _postcodeLocationClientService.LookupPostcode(SearchPostCode);
            
            CurrentLatitude = postcodesIoResponse.Result.Latitude;
            CurrentLongitude = postcodesIoResponse.Result.Longitude;
        }
        catch
        {
            // ignored
        }
    }
}
