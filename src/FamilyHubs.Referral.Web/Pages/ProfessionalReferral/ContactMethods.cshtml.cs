using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

//todo: check: check details -> here, clear -> error -> back.
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

        TextAreaValue = model.EngageReason;

        if (Errors == null)
            return;

        if (Errors.Contains(ProfessionalReferralError.ContactMethods_NothingEntered))
        {
            TextAreaValidationErrorMessage = "Enter how best to engage with this family";
        }
        if (Errors.Contains(ProfessionalReferralError.ContactMethods_TooLong))
        {
            TextAreaValidationErrorMessage = "How the service can engage with the family must be 500 characters or less";
            TextAreaValue = model.InvalidEngageReason;
        }
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        if (string.IsNullOrEmpty(TextAreaValue))
        {
            return RedirectToSelf(ProfessionalReferralError.ContactMethods_NothingEntered);
        }

        if (TextAreaValue.Length > 500)
        {
            // truncate at some large value, to stop a denial of service attack
            model.InvalidEngageReason = TextAreaValue?[..Math.Min(TextAreaValue.Length, 4500)];

            return RedirectToSelf(ProfessionalReferralError.ContactMethods_TooLong);
        }

        model.EngageReason = TextAreaValue;
        model.InvalidEngageReason = null;

        return NextPage();
    }
}