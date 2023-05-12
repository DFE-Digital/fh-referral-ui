using FamilyHubs.Referral.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Web.Models;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;
//todo: fix check details -> change contact methods change selection e.g. from text to phone, continue, then back and back again to check details & previous captured text has now been deleted
// need to clear not selected, either on post of last contact details page, or on get of check details page
// will mean if you eg. select phone, enter phone, go back, select text, enter text, got back, select phone again and it will have remembered the original phone, but think that's ok
public class TextModel : ProfessionalReferralSessionModel, ISingleTelephoneTextboxPageModel
{
    public string HeadingText { get; set; } = "";
    public string? HintText { get; set; }
    public string TextBoxLabel { get; set; } = "UK telephone number";
    public string ErrorText { get; set; } = "";

    [Required(ErrorMessage = "Enter a UK telephone number", AllowEmptyStrings = false)]
    [Phone(ErrorMessage = "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192")]
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

    protected override string? OnPostWithModel(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            ValidationValid = false;
            ErrorText = ModelState["TextBoxValue"]!.Errors[0].ErrorMessage;

            SetPageProperties(model);
            return null;
        }

        model.TextphoneNumber = TextBoxValue;

        return NextPage(ConnectContactDetailsJourneyPage.Textphone, model.ContactMethodsSelected);
    }

    private void SetPageProperties(ConnectionRequestModel model)
    {
        HeadingText = $"What telephone number should the service use to text {model.FamilyContactFullName}?";
        BackUrl = GenerateBackUrl(ConnectContactDetailsJourneyPage.Textphone, model.ContactMethodsSelected);
    }
}
