using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Web.Models;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class EmailModel : ProfessionalReferralCacheModel, ISingleEmailTextboxPageModel
{
    public string HeadingText { get; set; } = "";
    public string? HintText { get; set; }
    public string TextBoxLabel { get; set; } = "Email address";
    public string ErrorText { get; set; } = "Enter an email address in the correct format, like name@example.com";

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
        //todo: do we need the check?
        if (!string.IsNullOrEmpty(model.EmailAddress))
        {
            TextBoxValue = model.EmailAddress;
        }

        SetPageProperties(model);
    }

    protected override IActionResult OnPostWithModelNew(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            ValidationValid = false;
            SetPageProperties(model);
            return Page();
        }

        model.EmailAddress = TextBoxValue;

        //todo: fix this abomination
        return NextPage(NextPage(ConnectContactDetailsJourneyPage.Email, model.ContactMethodsSelected));
    }

    private void SetPageProperties(ConnectionRequestModel model)
    {
        HeadingText = $"What is the email address for {model.FamilyContactFullName}?";
        BackUrl = GenerateBackUrl(ConnectContactDetailsJourneyPage.Email, model.ContactMethodsSelected);
    }
}
