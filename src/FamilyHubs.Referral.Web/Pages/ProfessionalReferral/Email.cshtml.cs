using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Web.Models;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class EmailModel : ProfessionalReferralModel, ISingleEmailTextboxPageModel
{
    public string HeadingText { get; set; } = "";
    public string? HintText { get; set; }
    public string TextBoxLabel { get; set; } = "Email address";
    public string ErrorText { get; set; } = "Enter an email address in the correct format, like name@example.com";
    public bool ValidationValid { get; set; } = true;

    [Required]
    [EmailAddress]
    [StringLength(254, MinimumLength = 3)] // the EmailAddress attribute allows emails that are too long, so we have this too
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
