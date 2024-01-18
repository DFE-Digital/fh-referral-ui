using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.Referral.Core.DistributedCache;
using System.Web;
using FamilyHubs.SharedKernel.Razor.FullPages;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class EmailModel : ProfessionalReferralCacheModel, ISingleEmailTextboxPageModel
{
    public string HeadingText { get; set; } = "";
    public string? HintText { get; set; }
    public string TextBoxLabel { get; set; } = "Email address";
    public int? MaxLength => 254;

    [Required]
    [EmailAddress]
    [StringLength(254, MinimumLength = 3)] // the EmailAddress attribute allows emails that are too long, so we have this too
    [BindProperty]
    public string? TextBoxValue { get; set; }

    public EmailModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(ConnectJourneyPage.Email, connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        if (!HasErrors)
        {
            TextBoxValue = model.EmailAddress;
        }

        SetPageProperties(model);
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToSelf(null, ErrorId.Email_NotValid);
        }

        model.EmailAddress = TextBoxValue;

        return NextPage(ConnectContactDetailsJourneyPage.Email, model.ContactMethodsSelected);
    }

    private void SetPageProperties(ConnectionRequestModel model)
    {
        HeadingText = $"What is the email address for {HttpUtility.HtmlEncode(model.FamilyContactFullName)}?";
        BackUrl = GenerateBackUrl(ConnectContactDetailsJourneyPage.Email, model.ContactMethodsSelected);
    }
}
