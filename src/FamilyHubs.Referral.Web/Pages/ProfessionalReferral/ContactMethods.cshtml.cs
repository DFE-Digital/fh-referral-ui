using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ContactMethodsModel : ProfessionalReferralCacheModel, ITellTheServicePageModel
{
    public string DescriptionPartial => "/Pages/ProfessionalReferral/ContactMethodsContent.cshtml";
    public string? TextAreaValidationErrorMessage { get; set; }

    [BindProperty]
    public string? TextAreaValue { get; set; }

    public ContactMethodsModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(ConnectJourneyPage.ContactMethods, connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        BackUrl = GenerateBackUrl(ConnectContactDetailsJourneyPage.ContactMethods, model.ContactMethodsSelected);

        if (!HasErrors)
        {
            TextAreaValue = model.EngageReason;
            return;
        }

        if (model.ErrorState!.Errors.Contains(ProfessionalReferralError.ContactMethods_NothingEntered))
        {
            TextAreaValidationErrorMessage = "Enter how best to engage with this family";
        }
        else if (model.ErrorState!.Errors.Contains(ProfessionalReferralError.ContactMethods_TooLong))
        {
            TextAreaValidationErrorMessage = "How the service can engage with the family must be 500 characters or less";
            TextAreaValue = model.ErrorState!.InvalidUserInput!.First();
        }
        //todo: throw?
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        if (string.IsNullOrEmpty(TextAreaValue))
        {
            return RedirectToSelf(null,ProfessionalReferralError.ContactMethods_NothingEntered);
        }

        if (TextAreaValue.Length > 500)
        {
            return RedirectToSelf(TextAreaValue, ProfessionalReferralError.ContactMethods_TooLong);
        }

        model.EngageReason = TextAreaValue;

        return NextPage();
    }
}