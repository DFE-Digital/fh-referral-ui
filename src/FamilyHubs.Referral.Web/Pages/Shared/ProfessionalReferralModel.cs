using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Identity.Models;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;

namespace FamilyHubs.Referral.Web.Pages.Shared;

//todo: journey navigation gets messes up when have gone back to change contact methods, then back to check details, then back through the journey
// ^^ changing query param is in history and messes things up. move changing to cache instead and handle
//todo: use post redirect get pattern so that invalid pages don't ask for a reload (especially when using back)

public enum JourneyFlow
{
    Normal,
    ChangingPage,
    ChangingContactMethods
}

[Authorize(Roles = RoleGroups.LaProfessionalOrDualRole)]
public class ProfessionalReferralModel : HeaderPageModel
{
    protected readonly ConnectJourneyPage CurrentPage;
    protected IConnectionRequestDistributedCache ConnectionRequestCache { get; }
    // not set in ctor, but will always be there in Get/Set handlers
    public string ServiceId { get; set; } = default!;
    public JourneyFlow Flow { get; set; }
    public string? BackUrl { get; set; }
    // not set in ctor, but will always be there in Get/Post handlers
    public FamilyHubsUser ProfessionalUser { get; set; } = default!;

    public ProfessionalReferralModel(
        ConnectJourneyPage page,
        IConnectionRequestDistributedCache connectionRequestDistributedCache)
    {
        ConnectionRequestCache = connectionRequestDistributedCache;
        CurrentPage = page;
    }

    protected virtual Task<IActionResult> OnSafeGetAsync()
    {
        return Task.FromResult((IActionResult)Page());
    }

    protected virtual Task<IActionResult> OnSafePostAsync()
    {
        return Task.FromResult((IActionResult)Page());
    }

    public async Task<IActionResult> OnGetAsync(string serviceId, string? changing = null)
    {
        if (serviceId == null)
        {
            // someone's been monkeying with the query string and we don't have the service details we need
            // we can't send them back to the start of the journey because we don't know what service they were looking at
            // so we'll just send them to the home page
            return RedirectToPage("/Index");
        }

        ServiceId = serviceId;
        Flow = GetFlow(changing);

        // default, but can be overridden
        BackUrl = GenerateBackUrl(CurrentPage - 1);

        //todo: could do with a version that just gets the email address
        ProfessionalUser = HttpContext.GetFamilyHubsUser();

        return await OnSafeGetAsync();
    }

    protected JourneyFlow GetFlow(string? changing)
    {
        return changing switch
        {
            "page" => JourneyFlow.ChangingPage,
            "contact-methods" => JourneyFlow.ChangingContactMethods,
            _ => JourneyFlow.Normal
        };
    }

    protected string? GetChanging(JourneyFlow flow)
    {
        return flow switch
        {
            JourneyFlow.ChangingPage => "page",
            JourneyFlow.ChangingContactMethods => "contact-methods",
            _ => null
        };
    }

    public async Task<IActionResult> OnPostAsync(string serviceId, string? changing = null)
    {
        ServiceId = serviceId;

        Flow = GetFlow(changing);

        // default, but can be overridden
        BackUrl = GenerateBackUrl(CurrentPage - 1);

        ProfessionalUser = HttpContext.GetFamilyHubsUser();

        return await OnSafePostAsync();
    }

    class RouteValues
    {
        public string? ServiceId { get; set; }
        public string? Changing { get; set; }
    }

    protected IActionResult RedirectToProfessionalReferralPage(string page, string? changing = null)
    {
        return RedirectToPage($"/ProfessionalReferral/{page}", new RouteValues
        {
            ServiceId = ServiceId,
            Changing = changing            
        });
    }

    //todo: consts, if not an enum
    protected IActionResult NextPage(string? page = null)
    {
        page ??= (CurrentPage + 1).ToString();

        if (Flow == JourneyFlow.ChangingContactMethods)
        {
            return RedirectToPage($"/ProfessionalReferral/{page}", new
            {
                ServiceId,
                changing = "contact-methods"
            });
        }

        return RedirectToProfessionalReferralPage(
            Flow == JourneyFlow.ChangingPage ? "CheckDetails" : page);
    }

    //todo: better split between this and cache model
    protected string GenerateBackUrl(ConnectJourneyPage page)
    {
        ConnectJourneyPage? backUrlPage;
            
        if (Flow == JourneyFlow.ChangingContactMethods
            && page == ConnectJourneyPage.WhySupport) // ContactMethods-1
        {
            backUrlPage = ConnectJourneyPage.CheckDetails;
        }
        else
        {
            backUrlPage = Flow == JourneyFlow.ChangingPage ? ConnectJourneyPage.CheckDetails : page;
        }


        //todo: check ServiceId for null
        string url = $"/ProfessionalReferral/{backUrlPage}?ServiceId={ServiceId}";

        if (Flow == JourneyFlow.ChangingContactMethods
            && backUrlPage != ConnectJourneyPage.CheckDetails && backUrlPage != ConnectJourneyPage.WhySupport)
        {
            url = $"{url}&changing=contact-methods";
        }

        return url;
    }
}