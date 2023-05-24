using FamilyHubs.Referral.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyHubs.SharedKernel.Razor.FamilyHubsUi.Delegators;
using FamilyHubs.SharedKernel.Identity;
using FamilyHubs.SharedKernel.Identity.Models;

namespace FamilyHubs.Referral.Web.Pages.Shared;

//todo: use post redirect get pattern so that invalid pages don't ask for a reload (especially when using back)
//todo: current pattern will have to be extended when we need to keep existing user entries (possibly valid or not) 

public enum JourneyFlow
{
    Normal,
    ChangingPage,
    ChangingContactMethods
}

//todo: have only one of these
//todo: work into next page?
public enum ConnectJourneyPage
{
    LocalOfferDetail,
    Safeguarding,
    Consent,
    SupportDetails,
    WhySupport,
    ContactDetails,
    Email,
    Telephone,
    Text,
    Letter,
    ContactMethods,
    CheckDetails
}

[Authorize]
public class ProfessionalReferralModel : PageModel, IFamilyHubsHeader
{
    private readonly ConnectJourneyPage _page;
    // not set in ctor, but will always be there in Get/Set handlers
    public string ServiceId { get; set; } = default!;
    public JourneyFlow Flow { get; set; }
    public string? BackUrl { get; set; }
    public ProfessionalReferralError[]? Errors { get; set; }
    public bool ValidationValid { get; set; } = true;
    // not set in ctor, but will always be there in Get/Set handlers
    public FamilyHubsUser ProfessionalUser { get; set; } = default!;

    public ProfessionalReferralModel(ConnectJourneyPage page)
    {
        _page = page;
    }

    public bool ShowNavigationMenu => true;

    LinkStatus IFamilyHubsHeader.GetStatus(SharedKernel.Razor.FamilyHubsUi.Options.LinkOptions link)
    {
        return link.Text == "Search for service" ? LinkStatus.Active : LinkStatus.Visible;
    }

    protected virtual Task<IActionResult> OnSafeGetAsync()
    {
        if (Errors != null)
        {
            //todo: use Errors directly
            ValidationValid = false;
        }

        return Task.FromResult((IActionResult)Page());
    }

    protected virtual Task<IActionResult> OnSafePostAsync()
    {
        return Task.FromResult((IActionResult)Page());
    }

    public async Task<IActionResult> OnGetAsync(string serviceId, string? changing = null, string? errors = null)
    {
        if (serviceId == null)
        {
            // someone's been monkeying with the query string and we don't have the service details we need
            // we can't send them back to the start of the journey because we don't know what service they were looking at
            // so we'll just send them to the home page
            return RedirectToPage("/Index");
        }

        ServiceId = serviceId;
        //todo: do we want Property:error1, etc.? to generically set the link id?
        Errors = errors?.Split(',').Select(Enum.Parse<ProfessionalReferralError>).ToArray();

        Flow = GetFlow(changing);

        // default, but can be overridden
        BackUrl = GenerateBackUrl((_page-1).ToString());

        //todo: could do with a version that just gets the email address
        ProfessionalUser = HttpContext.GetFamilyHubsUser();

        return await OnSafeGetAsync();
    }

    private JourneyFlow GetFlow(string? changing)
    {
        return changing switch
        {
            "page" => JourneyFlow.ChangingPage,
            "contact-methods" => JourneyFlow.ChangingContactMethods,
            _ => JourneyFlow.Normal
        };
    }

    private string? GetChanging(JourneyFlow flow)
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
        BackUrl = GenerateBackUrl((_page-1).ToString());

        ProfessionalUser = HttpContext.GetFamilyHubsUser();

        return await OnSafePostAsync();
    }

    class RouteValues
    {
        public string? ServiceId { get; set; }
        public ProfessionalReferralError[]? Errors { get; set; }
        public string? Changing { get; set; }
    }

    protected IActionResult RedirectToSelf(params ProfessionalReferralError[] errors)
    {
        return RedirectToProfessionalReferralPage(_page.ToString(), GetChanging(Flow), errors);
    }

    protected IActionResult RedirectToProfessionalReferralPage(string page, string? changing = null, params ProfessionalReferralError[] errors)
    {
        return RedirectToPage($"/ProfessionalReferral/{page}", new RouteValues
        {
            ServiceId = ServiceId,
            Errors = errors,
            Changing = changing            
        });
    }

    //todo: consts, if not an enum
    protected IActionResult NextPage(string? page = null)
    {
        page ??= (_page + 1).ToString();

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

    //todo: work with enums until the last possible moment
    //todo: better split between this and session model
    protected string GenerateBackUrl(string page)
    {
        string? backUrlPage;
            
        if (Flow == JourneyFlow.ChangingContactMethods
            && page == ConnectJourneyPage.WhySupport.ToString()) // ContactMethods-1
        {
            backUrlPage = "CheckDetails";
        }
        else
        {
            backUrlPage = Flow == JourneyFlow.ChangingPage ? "CheckDetails" : page;
        }


        //todo: check ServiceId for null
        string url = $"/ProfessionalReferral/{backUrlPage}?ServiceId={ServiceId}";

        if (Flow == JourneyFlow.ChangingContactMethods
            && backUrlPage != "CheckDetails" && backUrlPage != "WhySupport")
        {
            url = $"{url}&changing=contact-methods";
        }

        return url;
    }
}