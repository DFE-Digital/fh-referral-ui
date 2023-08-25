using System.Text.RegularExpressions;
using FamilyHubs.Referral.Core.ApiClients;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

[Authorize]
public partial class SearchModel : HeaderPageModel
{
    [BindProperty]
    public string Postcode { get; set; } = default!;
    [BindProperty]
    public bool ValidationValid { get; set; } = true;
    public bool PostcodeValid { get; set; } = true;

    private readonly IPostcodeLocationClientService _postcodeLocationClientService;

    public SearchModel(IPostcodeLocationClientService postcodeLocationClientService)
    {
        _postcodeLocationClientService = postcodeLocationClientService;
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
                postcode = Postcode,
                currentPage = 1
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
