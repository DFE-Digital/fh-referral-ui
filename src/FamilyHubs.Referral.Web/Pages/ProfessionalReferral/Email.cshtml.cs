using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;
using FamilyHubs.Referral.Core.Helper;
using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Web.Models;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class EmailModel : ProfessionalReferralModel, ISingleTextboxPageModel
{
    //todo: get asp-for input to pick up is email

    public string ErrorId { get; set; } = "error-summary-title";
    public string HeadingText { get; set; } = "";
    public string? HintText { get; set; }
    public string TextBoxLabel { get; set; } = "Email address";
    public string MainErrorText { get; set; } = "Enter an email address in the correct format, like name@example.com";
    public string? TextBoxErrorText { get; set; } = "Enter an email address in the correct format, like name@example.com";
    public bool ValidationValid { get; set; } = true;

    //todo: check input matches gds email. partial doesn't find this as it uses PartialTextBoxViewModel instead
    [Required]
    [EmailAddress]
    [BindProperty]
    public string? TextBoxValue { get; set; }

    public EmailModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        HeadingText = $"What is the email address for {model.FamilyContactFullName}?";

        if (!string.IsNullOrEmpty(model.EmailAddress))
        {
            TextBoxValue = model.EmailAddress;
        }
    }

    protected override string? OnPostWithModel(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            ValidationValid = false;
            return null;
        }

        if (TextBoxValue!.Length > 255)
        {
            TextBoxValue = TextBoxValue.Truncate(252);
        }

        model.EmailAddress = TextBoxValue;

        string destination;
        if (model.TelephoneSelected)
        {
            //todo: const or route helper
            destination = "Telephone";
        }
        else if (model.TextphoneSelected)
        {
            destination = "Textphone";
        }
        else if (model.LetterSelected)
        {
            destination = "Letter";
        }
        else
        {
            destination = "ContactMethod";
        }

        return $"/ProfessionalReferral/{destination}";
    }
}
