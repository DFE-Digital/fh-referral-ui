using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ContactMethodsModel : ProfessionalReferralCacheModel, ITellTheServicePageModel
{
    public string DescriptionPartial => "/Pages/ProfessionalReferral/ContactMethodsContent.cshtml";

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

        if (Errors.HasTriggeredError((int)ErrorId.ContactMethods_TooLong))
        {
            TextAreaValue = model.ErrorState!.InvalidUserInput![0];
        }
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        if (string.IsNullOrEmpty(TextAreaValue))
        {
            return RedirectToSelf(null,ErrorId.ContactMethods_NothingEntered);
        }

        // workaround the front end counting line endings as 1 chars (\n) as per HTML spec,
        // and the http transport/.net/windows using 2 chars for line ends (\r\n)
        if (TextAreaValue.Replace("\r", "").Length > 500)
        {
            return RedirectToSelf(TextAreaValue, ErrorId.ContactMethods_TooLong);
        }

        model.EngageReason = TextAreaValue;

        return NextPage();
    }
}