using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.Shared;

//todo: use post redirect get pattern so that errored pages don't ask for a reload (especially when using back)

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

public class ProfessionalReferralModel : PageModel
{
    private readonly ConnectJourneyPage _page;
    public string? ServiceId { get; set; }
    public JourneyFlow Flow { get; set; }
    public string? BackUrl { get; set; }

    public ProfessionalReferralModel(ConnectJourneyPage page = ConnectJourneyPage.Safeguarding)
    {
        _page = page;
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
        BackUrl = GenerateBackUrl((_page-1).ToString());

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

    public async Task<IActionResult> OnPostAsync(string serviceId, string? changing = null)
    {
        ServiceId = serviceId;

        Flow = GetFlow(changing);

        // default, but can be overridden
        BackUrl = GenerateBackUrl((_page-1).ToString());

        return await OnSafePostAsync();
    }

    protected IActionResult RedirectToProfessionalReferralPage(string page)
    {
        return RedirectToPage($"/ProfessionalReferral/{page}", new
        {
            ServiceId
        });
    }

    //todo: consts, if not an enum
    protected IActionResult NextPage(string page)
    {
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