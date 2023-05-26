using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

//todo: redirect to get with error in url has the problem: error, next, back see error, rather than valid value
// either remove prg, or store the error in the cache and redirect to get without error in url
// (investigate 303?)
// clear error before redirecting to next page in post
// clear errors on get in check details

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

        if (ValidationValid)
            return;

        //todo: there are ways we could make this more generic and remove the need for pages to do this
        if (model.ErrorState!.Errors.Contains(ProfessionalReferralError.WhySupport_NothingEntered))
        {
            TextAreaValidationErrorMessage = "Enter a reason for the connection request";
        }
        if (model.ErrorState!.Errors.Contains(ProfessionalReferralError.WhySupport_TooLong))
        {
            TextAreaValidationErrorMessage = "Reason for the connection request must be 500 characters or less";
            TextAreaValue = model.ErrorState!.InvalidUserInput!.First();
        }
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        if (string.IsNullOrEmpty(TextAreaValue))
        {
            return RedirectToSelf(null, ProfessionalReferralError.WhySupport_NothingEntered);
        }

        if (TextAreaValue.Length > 500)
        {
            return RedirectToSelf(TextAreaValue, ProfessionalReferralError.WhySupport_TooLong);
        }

        model.Reason = TextAreaValue;

        return NextPage();
    }
}
