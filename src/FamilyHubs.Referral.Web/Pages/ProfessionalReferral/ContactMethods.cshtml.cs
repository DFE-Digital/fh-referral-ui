using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ContactMethodsModel : ProfessionalReferralSessionModel, ITellTheServicePageModel
{
    public string DescriptionPartial => "/Pages/ProfessionalReferral/ContactMethodsContent.cshtml";

    [BindProperty]
    public string? TextAreaValue { get; set; }

    public string? TextAreaValidationErrorMessage { get; set; }

    public ContactMethodsModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(ConnectJourneyPage.ContactMethods, connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        SetPageProperties(model);

        //todo: need the check??
        if (!string.IsNullOrEmpty(model.EngageReason))
            TextAreaValue = model.EngageReason;
    }

    protected override string? OnPostWithModel(ConnectionRequestModel model)
    {
        if (string.IsNullOrEmpty(TextAreaValue))
        {
            SetPageProperties(model);
            TextAreaValidationErrorMessage = "Enter how best to engage with this family";
            return null;
        }

        if (TextAreaValue.Length > 500)
        {
            SetPageProperties(model);
            TextAreaValidationErrorMessage = "How the service can engage with the family must be 500 characters or less";
            return null;
        }

        model.EngageReason = TextAreaValue;

        //todo: use next page
        return "CheckDetails";
    }

    private void SetPageProperties(ConnectionRequestModel model)
    {
        BackUrl = GenerateBackUrl(ConnectContactDetailsJourneyPage.ContactMethods, model.ContactMethodsSelected);
    }
}