using FamilyHubs.ReferralUi.Ui.Models;
using FamilyHubs.ReferralUi.Ui.Services.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace FamilyHubs.ReferralUi.Ui.Pages.ProfessionalReferral;

public partial class SearchModel : PageModel
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
        //Standard GET method for page
    }

    public async Task<IActionResult> OnPost()
    {
        var validPostcode = PostcodeRegex();
        if (string.IsNullOrEmpty(Postcode))
        {
            PostcodeValid = false;
            return Page();
        }
        if(!validPostcode.IsMatch(Postcode))
        {
            ValidationValid = false;
            return Page();
        }
        try
        {
            await _postcodeLocationClientService.LookupPostcode(Postcode);

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

    [GeneratedRegex("([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\\s?[0-9][A-Za-z]{2})")]
    private static partial Regex PostcodeRegex();
}
