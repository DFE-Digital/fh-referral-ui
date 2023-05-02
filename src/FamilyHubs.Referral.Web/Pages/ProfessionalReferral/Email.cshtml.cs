using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;
using FamilyHubs.Referral.Core.Helper;
using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Web.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        //todo: don't truncate email, it will invalidate it
        // check inbuilt validation limits it to max email length
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
