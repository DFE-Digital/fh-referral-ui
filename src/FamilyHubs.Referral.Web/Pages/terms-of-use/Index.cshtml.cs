using FamilyHubs.SharedKernel.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.terms_of_use;

public class IndexModel : PageModel
{
    private readonly ITermsAndConditionsService _termsAndConditionsService;

    [BindProperty]
    public string? ReturnPath { get; set; }

    public IndexModel(ITermsAndConditionsService termsAndConditionsService)
    {
        _termsAndConditionsService = termsAndConditionsService;
    }

    public void OnGet(string returnPath)
    {
        ReturnPath = returnPath;
    }

    public async Task<IActionResult> OnPost()
    {
        await _termsAndConditionsService.AcceptTermsAndConditions();
        return Redirect(ReturnPath!);
    }
}