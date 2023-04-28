using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;
using FamilyHubs.Referral.Core.Helper;
using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.Referral.Core.DistributedCache;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class EmailModel : ProfessionalReferralModel
{
    public PartialTextBoxViewModel PartialTextBoxViewModel { get; set; } = new()
    {
        HeadingText = string.Empty,
        ErrorId = "error-summary-title",
        HintText = string.Empty,
        TextBoxLabel = "Email address",
        MainErrorText = "Enter an email address in the correct format, like name@example.com",
        TextBoxErrorText = "Enter an email address in the correct format, like name@example.com",
    };

    //todo: we're not using client side validation. does this cause the model to be invalid? how is that handled?
    [EmailAddress]
    [BindProperty]
    public string TextBoxValue { get; set; } = string.Empty;

    public EmailModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        PartialTextBoxViewModel.HeadingText = $"What is the email address for {model.FamilyContactFullName}?";

        if (!string.IsNullOrEmpty(model.EmailAddress))
        {
            PartialTextBoxViewModel.TextBoxValue = model.EmailAddress;
            TextBoxValue = model.EmailAddress;
        }
    }

    protected override string? OnPostWithModel(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid || string.IsNullOrEmpty(TextBoxValue))
        {
            PartialTextBoxViewModel.TextBoxValue = TextBoxValue;
            PartialTextBoxViewModel.ValidationValid = false;

            return null;
        }

        if (TextBoxValue.Length > 255)
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
