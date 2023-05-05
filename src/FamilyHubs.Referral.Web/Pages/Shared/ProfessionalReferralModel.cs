using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.Shared;

public abstract class ProfessionalReferralModel : PageModel
{
    [BindProperty]
    public string? ServiceId { get; set; }

    [BindProperty]
    public string? ServiceName { get; set; }

    public bool ValidationValid { get; set; } = true;

    protected readonly IConnectionRequestDistributedCache ConnectionRequestCache;

    protected ProfessionalReferralModel(IConnectionRequestDistributedCache connectionRequestCache)
    {
        ConnectionRequestCache = connectionRequestCache;
    }

    protected abstract void OnGetWithModel(ConnectionRequestModel model);
    protected abstract string? OnPostWithModel(ConnectionRequestModel model);

    public async Task<IActionResult> OnGetAsync(string serviceId)
    {
        var model = await ConnectionRequestCache.GetAsync();
        if (model == null)
        {
            // session has expired and we don't have a model to work with
            // likely the user has come back to this page after a long time
            // send them back to the start of the journey
            return RedirectToPage("/ProfessionalReferral/LocalOfferDetail", new { serviceId });
        }

        ServiceId = model.ServiceId;
        ServiceName = model.ServiceName;

        OnGetWithModel(model);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var model = await ConnectionRequestCache.GetAsync();
        if (model == null)
        {
            // session has expired and we don't have a model to work with
            // likely the user has come back to this page after a long time
            // send them back to the start of the journey
            return RedirectToPage("/ProfessionalReferral/LocalOfferDetail", new { ServiceId });
        }

        string? nextPage = OnPostWithModel(model);
        if (nextPage == null)
        {
            return Page();
        }

        await ConnectionRequestCache.SetAsync(model);

        return RedirectToPage(nextPage, new
        {
            ServiceId,
            ServiceName
        });
    }

    private static string[] _connectJourneyPages =
    {
        "ContactDetails",
        "Email",
        "Telephone",
        "Text",
        "Letter",
        "ContactMethods"
    };

    protected string FirstContactMethodPage(bool[] contactMethodsSelected)
    {
        return NextPage((ContactMethod)(-1), contactMethodsSelected);
    }

    protected string NextPage(ContactMethod currentPage, bool[] contactMethodsSelected)
    {
        while (++currentPage <= ContactMethod.Last)
        {
            if (contactMethodsSelected[(int) currentPage])
            {
                break;
            }
        }

        return $"/ProfessionalReferral/{_connectJourneyPages[(int)currentPage+1]}";
    }

    protected string PreviousPage(ContactMethod currentPage, bool[] contactMethodsSelected)
    {
        while (--currentPage >= 0)
        {
            if (contactMethodsSelected[(int)currentPage])
            {
                break;
            }
        }

        return $"/ProfessionalReferral/{_connectJourneyPages[(int)currentPage + 1]}";
    }
}