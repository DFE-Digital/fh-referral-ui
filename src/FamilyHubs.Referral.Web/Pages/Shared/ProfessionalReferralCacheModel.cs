using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Errors;
using FamilyHubs.SharedKernel.Razor.ErrorNext;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.Shared;

public class ProfessionalReferralCacheModel : ProfessionalReferralModel
{
    // we could stop passing this to get/set
    public ConnectionRequestModel? ConnectionRequestModel { get; set; }
    public IErrorState Errors { get; protected set; }
    private bool _redirectingToSelf;

    protected ProfessionalReferralCacheModel(
        ConnectJourneyPage page,
        IConnectionRequestDistributedCache connectionRequestCache)
        : base(page, connectionRequestCache)
    {
        Errors = ErrorState.Empty;
    }

    //todo: change to private set
    //todo: remove this and reference Errors directly
    public bool HasErrors { get; set; }

    protected virtual void OnGetWithModel(ConnectionRequestModel model)
    {
    }

    //todo: these (get/post) should be passing though CancellationToken
    protected virtual Task OnGetWithModelAsync(ConnectionRequestModel model)
    {
        OnGetWithModel(model);

        return Task.CompletedTask;
    }

    protected virtual IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        return Page();
    }

    protected virtual Task<IActionResult> OnPostWithModelAsync(ConnectionRequestModel model)
    {
        return Task.FromResult(OnPostWithModel(model));
    }

    protected override async Task<IActionResult> OnSafeGetAsync()
    {
        ConnectionRequestModel = await ConnectionRequestCache.GetAsync(ProfessionalUser.Email);
        if (ConnectionRequestModel == null)
        {
            // the journey cache entry has expired and we don't have a model to work with
            // likely the user has come back to this page after a long time
            // send them back to the start of the journey
            // not strictly a journey page, but still works
            return RedirectToProfessionalReferralPage("LocalOfferDetail");
        }

        if (ConnectionRequestModel.ErrorState?.ErrorPage == CurrentPage)
        {
            HasErrors = true;
        }
        else
        {
            // we don't save the model on Get, but we don't want the page to pick up the error state when the user has gone back
            // (we'll clear the error state in the model on a non-redirect to self post
            ConnectionRequestModel.ErrorState = null;
        }

        Errors = ErrorState.Create(PossibleErrors.All, ConnectionRequestModel.ErrorState?.Errors ?? Array.Empty<ErrorId>());

        await OnGetWithModelAsync(ConnectionRequestModel);

        return Page();
    }

    protected override async Task<IActionResult> OnSafePostAsync()
    {
        ConnectionRequestModel = await ConnectionRequestCache.GetAsync(ProfessionalUser.Email);
        if (ConnectionRequestModel == null)
        {
            // the journey cache entry has expired and we don't have a model to work with
            // likely the user has come back to this page after a long time
            // send them back to the start of the journey
            return RedirectToProfessionalReferralPage("LocalOfferDetail");
        }

        var result = await OnPostWithModelAsync(ConnectionRequestModel);

        if (!_redirectingToSelf)
        {
            ConnectionRequestModel.ErrorState = null;
        }

        await ConnectionRequestCache.SetAsync(ProfessionalUser.Email, ConnectionRequestModel);

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

    protected IActionResult FirstContactMethodPage(bool[] contactMethodsSelected)
    {
        return NextPage((ConnectContactDetailsJourneyPage)(-1), contactMethodsSelected);
    }

    protected IActionResult NextPage(ConnectContactDetailsJourneyPage currentPage, bool[] contactMethodsSelected)
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
            return NextPage("CheckDetails");
        }

        return NextPage(_connectJourneyPages[(int)currentPage+1]);
    }

    protected string GenerateBackUrl(ConnectContactDetailsJourneyPage currentPage, bool[] contactMethodsSelected)
    {
        return GenerateBackUrl(PreviousPage(currentPage, contactMethodsSelected));
    }

    private ConnectJourneyPage PreviousPage(ConnectContactDetailsJourneyPage currentPage, bool[] contactMethodsSelected)
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

        return (ConnectJourneyPage)(currentPage + (int)ConnectJourneyPage.Email);
    }

    //todo: version that accepts array of user input
    protected IActionResult RedirectToSelf(string? invalidUserInput, params ErrorId[] errors)
    {
        //todo: throw if none? is that something this should be used for?
        if (errors.Any())
        {
            // truncate at some large value, to stop a denial of service attack
            var safeInvalidUserInput = invalidUserInput != null
                ? new[] {invalidUserInput[..Math.Min(invalidUserInput.Length, 4500)]}
                : null;

            //todo: throw if model null?
            ConnectionRequestModel!.ErrorState =
                new ProfessionalReferralErrorState(CurrentPage, errors, safeInvalidUserInput);
        }

        _redirectingToSelf = true;

        return RedirectToProfessionalReferralPage(CurrentPage.ToString(), GetChanging(Flow));
    }
}