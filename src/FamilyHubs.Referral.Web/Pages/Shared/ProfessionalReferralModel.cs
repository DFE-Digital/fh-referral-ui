using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHubs.Referral.Web.Pages.Shared;

//todo: back to errored page asked to resubmit form??

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

        // default, but can be overridden
        BackUrl = GenerateBackUrl((_page-1).ToString());

        return await OnSafeGetAsync();
    }

    public async Task<IActionResult> OnPostAsync(string serviceId, string? changing = null)
    {
        ServiceId = serviceId;

        // default, but can be overridden
        BackUrl = GenerateBackUrl((_page-1).ToString());

        Flow = changing switch
        {
            "page" => JourneyFlow.ChangingPage,
            "contact-methods" => JourneyFlow.ChangingContactMethods,
            _ => JourneyFlow.Normal
        };

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

    //protected string PreviousPage(string page)
    //{
    //    return Flow == JourneyFlow.ChangingPage ? "CheckDetails" : page;
    //}

    //todo: better split between this and session model
    protected string GenerateBackUrl(string page)
    {
        string backUrlPage = Flow == JourneyFlow.ChangingPage ? "CheckDetails" : page;

        dynamic dynamicObject = new System.Dynamic.ExpandoObject();
        //todo: check for null
        dynamicObject.ServiceId = ServiceId;

        if (Flow == JourneyFlow.ChangingContactMethods)
        {
            dynamicObject.changing = "contact-methods";
        }

        //todo: unit testing BackUrl when it uses this is going to be a pain. just do it manually instead?
        return UrlHelperExtensions.Page(Url, $"/ProfessionalReferral/{backUrlPage}", dynamicObject);
    }
}