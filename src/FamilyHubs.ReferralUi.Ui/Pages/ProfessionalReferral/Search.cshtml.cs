using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

[Authorize(Policy = "Referrer")]
public class SearchModel : PageModel
{
    [BindProperty]
    public string SearchOption { get; set; } = default!;

    [BindProperty]
    public string ServiceName { get; set; } = default!;

    [BindProperty]
    public string Postcode { get; set; } = default!;
    [BindProperty]
    public bool ValidationValid { get; set; } = true;
    public bool ServiceNameValid { get; set; } = true;
    public bool PostcodeValid { get; set; } = true;

    private readonly IPostcodeLocationClientService _postcodeLocationClientService;

    public SearchModel(IPostcodeLocationClientService postcodeLocationClientService)
    {
        _postcodeLocationClientService = postcodeLocationClientService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
        switch(SearchOption)
        {
            case "name":
                ModelState.Remove("Postcode");
                if (string.IsNullOrEmpty(ServiceName))
                    ServiceNameValid = false;
                break;

            case "postcode":
                ModelState.Remove("ServiceName");
                if (string.IsNullOrEmpty(Postcode))
                    PostcodeValid = false;
                break;

            default:
                ValidationValid = false;
                return Page();
        }

        if (!ModelState.IsValid)
        {
            ValidationValid = false;
            return Page();
        }

        if (string.Compare(SearchOption, "Name", StringComparison.OrdinalIgnoreCase) == 0)
        {
            return RedirectToPage("LocalOfferResults", new
            {
                latitude = 0.0D,
                longitude = 0.0D,
                distance = 0.0D,
                minimumAge = 0,
                maximumAge = 99,
                searchText = ServiceName
            });

        }

        try
        {
            PostcodeApiModel postcodeApiModel = await _postcodeLocationClientService.LookupPostcode(Postcode);

            return RedirectToPage("LocalOfferResults", new 
            {
                Postcode,
                //distance = 32186.9 //212892.0
            });
        }
        catch
        {
            ValidationValid = false;
            return Page();
        }

    }
}
