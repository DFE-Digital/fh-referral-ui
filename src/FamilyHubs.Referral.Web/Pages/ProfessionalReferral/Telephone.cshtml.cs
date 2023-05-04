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
    public string ErrorText { get; set; } = "";
    public bool ValidationValid { get; set; } = true;
    public string? BackUrl { get; set; }

    [Required(ErrorMessage = "Enter a UK telephone number", AllowEmptyStrings = false)]
    [Phone(ErrorMessage = "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192")]
    [BindProperty]
    public string? TextBoxValue { get; set; }

    public TelephoneModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        if (!string.IsNullOrEmpty(model.TelephoneNumber))
        {
            TextBoxValue = model.TelephoneNumber;
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

        model.TelephoneNumber = TextBoxValue;

        //string destination;
        //if (model.TextphoneSelected)
        //{
        //    destination = "Textphone";
        //}
        //else if (model.LetterSelected)
        //{
        //    destination = "Letter";
        //}
        //else
        //{
        //    destination = "ContactMethod";
        //}

        //return $"/ProfessionalReferral/{destination}";

        return NextPage(ContactMethod.Telephone, model.ContactMethodsSelected);
    }

    //private string GetBackUrl(bool emailSelected)
    //{
    //    return $"/ProfessionalReferral/{(emailSelected?"Email": "ContactDetails")}";
    //}

    private void SetPageProperties(ConnectionRequestModel model)
    {
        HeadingText = $"What telephone number should the service use to call {model.FamilyContactFullName}?";
        BackUrl = PreviousPage(ContactMethod.Telephone, model.ContactMethodsSelected);
    }
}
