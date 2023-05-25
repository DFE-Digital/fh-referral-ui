using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.Shared;

public abstract class ProfessionalReferralCacheModel : ProfessionalReferralModel
{
    protected ProfessionalReferralCacheModel(
        ConnectJourneyPage page,
        IConnectionRequestDistributedCache connectionRequestCache)
        : base(connectionRequestCache, page)
    {
    }

    protected abstract void OnGetWithModel(ConnectionRequestModel model);

    protected virtual string? OnPostWithModel(ConnectionRequestModel model)
    {
        // this is only here while we evolve the code, and is not expected to be called
        throw new NotImplementedException();
    }

    protected virtual IActionResult OnPostWithModelNew(ConnectionRequestModel model)
    {
        string? nextPage = OnPostWithModel(model);
        if (nextPage == null)
        {
            return Page();
        }

        //todo: change NextPage to return a Task, or add a NextPageAsync
        return NextPage(nextPage);
    }

    //todo: move all over to this one, then remove the above and rename this to OnPostWithModel
    // consumers will call NextPage() instead of returning a string (and we can pick up the page from the model)
    protected virtual Task<IActionResult> OnPostWithModelNewAsync(ConnectionRequestModel model)
    {
        return Task.FromResult(OnPostWithModelNew(model));
    }

    protected override async Task<IActionResult> OnSafeGetAsync()
    {
        var model = await ConnectionRequestCache.GetAsync(ProfessionalUser.Email);
        if (model == null)
        {
            // the journey cache entry has expired and we don't have a model to work with
            // likely the user has come back to this page after a long time
            // send them back to the start of the journey
            // not strictly a journey page, but still works
            return RedirectToProfessionalReferralPage("LocalOfferDetail");
        }

        OnGetWithModel(model);

        return Page();
    }

    protected override async Task<IActionResult> OnSafePostAsync()
    {
        var model = await ConnectionRequestCache.GetAsync(ProfessionalUser.Email);
        if (model == null)
        {
            // the journey cache entry has expired and we don't have a model to work with
            // likely the user has come back to this page after a long time
            // send them back to the start of the journey
            return RedirectToProfessionalReferralPage("LocalOfferDetail");
        }

        var result = await OnPostWithModelNewAsync(model);

        await ConnectionRequestCache.SetAsync(ProfessionalUser.Email, model);

        return result;
    }

    //todo: probably want to move these into the base?
    //todo: once enums merged, just use tostring instead
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
        return NextPage((ConnectContactDetailsJourneyPage)(-1), contactMethodsSelected);
    }

    protected string NextPage(ConnectContactDetailsJourneyPage currentPage, bool[] contactMethodsSelected)
    {
        // we could do this, but should be handled later anyway
        //if (Flow == JourneyFlow.ChangingPage)
        //{
        //    return "CheckDetails";
        //}

        while (++currentPage <= ConnectContactDetailsJourneyPage.LastContactMethod)
        {
            if (contactMethodsSelected[(int) currentPage])
            {
                break;
            }
        }

        if (Flow == JourneyFlow.ChangingContactMethods
            && currentPage == ConnectContactDetailsJourneyPage.ContactMethods)
        {
            return "CheckDetails";
        }

        return _connectJourneyPages[(int)currentPage+1];
    }

    protected string GenerateBackUrl(ConnectContactDetailsJourneyPage currentPage, bool[] contactMethodsSelected)
    {
        return GenerateBackUrl(PreviousPage(currentPage, contactMethodsSelected));
    }

    private string PreviousPage(ConnectContactDetailsJourneyPage currentPage, bool[] contactMethodsSelected)
    {
        while (--currentPage >= 0)
        {
            if (contactMethodsSelected[(int)currentPage])
            {
                break;
            }
        }

        //if (Flow == JourneyFlow.ChangingContactMethods
        //    && currentPage == ConnectContactDetailsJourneyPage.ContactMethods)
        //{
        //    return "CheckDetails";
        //}

        return _connectJourneyPages[(int)currentPage + 1];
    }
}