using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Services.Postcode.Interfaces;
using FamilyHubs.SharedKernel.Services.Postcode.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

//todo: it would be better to look up and store the postcode once here, rather than each time on the results page

[Authorize(Roles = RoleGroups.LaOrVcsProfessionalOrDualRole)]
public class SearchModel : HeaderPageModel
{
    private readonly IPostcodeLookup _postcodeLookup;

    [BindProperty]
    public string? Postcode { get; set; }

    public bool PostcodeValid { get; set; } = true;

    public SearchModel(IPostcodeLookup postcodeLookup)
    {
        _postcodeLookup = postcodeLookup;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var (postcodeError, _) = await _postcodeLookup.Get(Postcode);
        if (postcodeError == PostcodeError.None)
        {
            return RedirectToPage("LocalOfferResults", new
            {
                postcode = Postcode,
                currentPage = 1
            });
        }

        PostcodeValid = false;
        return Page();
    }
}
