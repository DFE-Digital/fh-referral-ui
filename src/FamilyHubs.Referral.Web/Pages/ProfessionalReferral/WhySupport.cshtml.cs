using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

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
        if (!HasErrors)
        {
            TextAreaValue = model.Reason;
            return;
        }

        //todo: there are ways we could make this more generic and remove the need for pages to do this
        if (model.ErrorState!.Errors.Contains(ProfessionalReferralError.WhySupport_NothingEntered))
        {
            TextAreaValidationErrorMessage = "Enter a reason for the connection request";
        }
        else if (model.ErrorState!.Errors.Contains(ProfessionalReferralError.WhySupport_TooLong))
        {
            TextAreaValidationErrorMessage = "Reason for the connection request must be 500 characters or less";
            TextAreaValue = model.ErrorState!.InvalidUserInput!.First();
        }
        //todo: throw?
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
