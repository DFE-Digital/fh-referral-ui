using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class WhySupportModel : ProfessionalReferralCacheModel, ITellTheServicePageModel
{
    public string DescriptionPartial => "/Pages/ProfessionalReferral/WhySupportContent.cshtml";
    public string? TextAreaValidationErrorMessage { get; set; }

    [BindProperty]
    public string? TextAreaValue { get; set; }

    public WhySupportModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(ConnectJourneyPage.WhySupport, connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        TextAreaValue = model.Reason;

        if (Errors == null)
            return;

        //todo: there are ways we could make this more generic and remove the need for pages to do this
        if (Errors.Contains(ProfessionalReferralError.WhySupport_NothingEntered))
        {
            TextAreaValidationErrorMessage = "Enter a reason for the connection request";
        }
        if (Errors.Contains(ProfessionalReferralError.WhySupport_TooLong))
        {
            TextAreaValidationErrorMessage = "Reason for the connection request must be 500 characters or less";
        }
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        //todo: truncate at some (large) value, to stop a denial of service attack
        model.Reason = TextAreaValue;

        if (string.IsNullOrEmpty(TextAreaValue))
        {
            return RedirectToSelf(ProfessionalReferralError.WhySupport_NothingEntered);
        }

        if (TextAreaValue.Length > 500)
        {
            return RedirectToSelf(ProfessionalReferralError.WhySupport_TooLong);
        }

        return NextPage();
    }
}
