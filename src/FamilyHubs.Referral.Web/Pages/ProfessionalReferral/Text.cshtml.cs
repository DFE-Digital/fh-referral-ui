using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Core.ValidationAttributes;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class TextModel : ProfessionalReferralCacheModel, ISingleTelephoneTextboxPageModel
{
    public string HeadingText { get; set; } = "";
    public string? HintText { get; set; }
    public string TextBoxLabel { get; set; } = "UK telephone number";
    public string ErrorText { get; set; } = "";

    [Required(ErrorMessage = "Enter a UK telephone number", AllowEmptyStrings = false)]
    [UkGdsTelephoneNumber]
    [BindProperty]
    public string? TextBoxValue { get; set; }

    public TextModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(ConnectJourneyPage.Text, connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        if (!string.IsNullOrEmpty(model.TextphoneNumber))
        {
            TextBoxValue = model.TextphoneNumber;
        }

        SetPageProperties(model);
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            ValidationValid = false;
            ErrorText = ModelState["TextBoxValue"]!.Errors[0].ErrorMessage;

            SetPageProperties(model);
            return Page();
        }

        model.TextphoneNumber = TextBoxValue;

        return NextPage(NextPage(ConnectContactDetailsJourneyPage.Textphone, model.ContactMethodsSelected));
    }

    private void SetPageProperties(ConnectionRequestModel model)
    {
        HeadingText = $"What telephone number should the service use to text {model.FamilyContactFullName}?";
        BackUrl = GenerateBackUrl(ConnectContactDetailsJourneyPage.Textphone, model.ContactMethodsSelected);
    }
}
