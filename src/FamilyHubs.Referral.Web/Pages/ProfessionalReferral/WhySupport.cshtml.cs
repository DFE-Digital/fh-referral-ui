using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

//todo: redirect to get with error in url has the problem error, next, back see error, rather than valid value
// either remove prg, or store the error in the cache and redirect to get without error in url

//todo: could have new base class for TellTheService pages (this and ContactMethodsModel)
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
            TextAreaValue = model.InvalidReason;
        }
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        if (string.IsNullOrEmpty(TextAreaValue))
        {
            return RedirectToSelf(ProfessionalReferralError.WhySupport_NothingEntered);
        }

        if (TextAreaValue.Length > 500)
        {
            // truncate at some large value, to stop a denial of service attack
            model.InvalidReason = TextAreaValue?[..Math.Min(TextAreaValue.Length, 4500)];

            return RedirectToSelf(ProfessionalReferralError.WhySupport_TooLong);
        }

        model.Reason = TextAreaValue;
        model.InvalidReason = null;

        return NextPage();
    }
}
