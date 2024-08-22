using System.Collections.Immutable;
using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.ValidationAttributes;
using System.Web;
using FamilyHubs.SharedKernel.Razor.ErrorNext;
using FamilyHubs.SharedKernel.Razor.FullPages.SingleTextbox;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class TextModel : ProfessionalReferralCacheModel, ISingleTelephoneTextboxPageModel
{
    public string HeadingText { get; set; } = "";
    public string? HintText { get; set; }
    public string TextBoxLabel { get; set; } = "UK telephone number";
    public int? MaxLength => 64;

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
            HasErrors = true;

            var errors = ImmutableDictionary
                .Create<int, PossibleError>()
                .Add(AdHocErrorId.Error1, ModelState["TextBoxValue"]!.Errors[0].ErrorMessage);

            Errors = ErrorState.Create(errors, AdHocErrorId.Error1);

            SetPageProperties(model);
            return Page();
        }

        model.TextphoneNumber = TextBoxValue;

        return NextPage(ConnectContactDetailsJourneyPage.Textphone, model.ContactMethodsSelected);
    }

    private void SetPageProperties(ConnectionRequestModel model)
    {
        HeadingText = $"What telephone number should this service use to text {HttpUtility.HtmlEncode(model.FamilyContactFullName)}?";
        BackUrl = GenerateBackUrl(ConnectContactDetailsJourneyPage.Textphone, model.ContactMethodsSelected);
    }
}
