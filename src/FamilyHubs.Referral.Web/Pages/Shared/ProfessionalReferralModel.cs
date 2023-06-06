using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Delegators;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Identity.Models;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;

namespace FamilyHubs.Referral.Web.Pages.Shared;

//todo: journey navigation gets messes up when have gone back to change contact methods, then back to check details, then back through the journey
// ^^ changing query param is in history and messes things up. move changing to cache instead and handle
//todo: use post redirect get pattern so that invalid pages don't ask for a reload (especially when using back)

[Authorize]
public class ProfessionalReferralModel : PageModel, IFamilyHubsHeader
{
    protected readonly ConnectJourneyPage CurrentPage;

    protected IConnectionRequestDistributedCache ConnectionRequestCache { get; }

    // not set in ctor, but will always be there in Get/Set handlers
    public string ServiceId { get; set; } = default!;

    public string? BackUrl { get; set; }

    // not set in ctor, but will always be there in Get/Set handlers
    public FamilyHubsUser ProfessionalUser { get; set; } = default!;

    public ProfessionalReferralModel(
        IConnectionRequestDistributedCache connectionRequestDistributedCache,
        ConnectJourneyPage page)
    {
        ConnectionRequestCache = connectionRequestDistributedCache;
        CurrentPage = page;
    }

    public bool ShowNavigationMenu => true;

    LinkStatus IFamilyHubsHeader.GetStatus(SharedKernel.Razor.FamilyHubsUi.Options.LinkOptions link)
    {
        return link.Text == "Search for service" ? LinkStatus.Active : LinkStatus.Visible;
    }

    protected virtual Task<IActionResult> OnSafeGetAsync()
    {
        return Task.FromResult((IActionResult) Page());
    }

    protected virtual Task<IActionResult> OnSafePostAsync()
    {
        return Task.FromResult((IActionResult) Page());
    }

    public async Task<IActionResult> OnGetAsync(string serviceId)
    {
        if (serviceId == null)
        {
            // someone's been monkeying with the query string and we don't have the service details we need
            // we can't send them back to the start of the journey because we don't know what service they were looking at
            // so we'll just send them to the home page
            return RedirectToPage("/Index");
        }

        ServiceId = serviceId;
        //todo: in derived
        //Flow = GetFlow(changing);

        //todo: in derived
        // default, but can be overridden
        BackUrl = GenerateBackUrl(CurrentPage - 1);

        //todo: could do with a version that just gets the email address
        ProfessionalUser = HttpContext.GetFamilyHubsUser();

        return await OnSafeGetAsync();
    }

    //protected JourneyFlow GetFlow(string? changing)
    //{
    //    return changing switch
    //    {
    //        "page" => JourneyFlow.ChangingPage,
    //        "contact-methods" => JourneyFlow.ChangingContactMethods,
    //        _ => JourneyFlow.Normal
    //    };
    //}

    //protected string? GetChanging(JourneyFlow flow)
    //{
    //    return flow switch
    //    {
    //        JourneyFlow.ChangingPage => "page",
    //        JourneyFlow.ChangingContactMethods => "contact-methods",
    //        _ => null
    //    };
    //}

    public async Task<IActionResult> OnPostAsync(string serviceId)
    {
        ServiceId = serviceId;

        //todo: in derived
        //Flow = GetFlow(changing);

        //todo: in derived
        // default, but can be overridden
        BackUrl = GenerateBackUrl(CurrentPage - 1);

        ProfessionalUser = HttpContext.GetFamilyHubsUser();

        return await OnSafePostAsync();
    }

    //todo: accept enum
    protected IActionResult RedirectToProfessionalReferralPage(string page)
    {
        return RedirectToPage($"/ProfessionalReferral/{page}", new
        {
            ServiceId
        });
    }

    protected virtual IActionResult NextPage()
    {
        return RedirectToProfessionalReferralPage((CurrentPage + 1).ToString());
    }

    protected virtual string GenerateBackUrl(ConnectJourneyPage page)
    {
        //todo: check ServiceId for null
        return $"/ProfessionalReferral/{page}?ServiceId={ServiceId}";
    }
}