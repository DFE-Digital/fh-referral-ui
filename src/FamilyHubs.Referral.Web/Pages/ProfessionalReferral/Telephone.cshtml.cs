using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Web.Models;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class TelephoneModel : ProfessionalReferralModel, ISingleTelephoneTextboxPageModel
{
    public string HeadingText { get; set; } = "";
    public string? HintText { get; set; }
    public string TextBoxLabel { get; set; } = "UK telephone number";
    //todo: will need different error texts
    // Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192
    public string ErrorText { get; set; } = "Enter a UK telephone number";
    public bool ValidationValid { get; set; } = true;

    [Required]
    [Phone]
    [BindProperty]
    public string? TextBoxValue { get; set; }

    public TelephoneModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        HeadingText = $"What telephone number should the service use to call {model.FamilyContactFullName}?";

        if (!string.IsNullOrEmpty(model.TelephoneNumber))
        {
            TextBoxValue = model.TelephoneNumber;
        }
    }

    protected override string? OnPostWithModel(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            ValidationValid = false;
            return null;
        }

        model.TelephoneNumber = TextBoxValue;

        string destination;
        if (model.TextphoneSelected)
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
