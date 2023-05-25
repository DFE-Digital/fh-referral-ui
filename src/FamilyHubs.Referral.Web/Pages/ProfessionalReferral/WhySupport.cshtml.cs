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
        if (!string.IsNullOrEmpty(model.Reason))
            TextAreaValue = model.Reason;
    }

    protected override Task<IActionResult> OnPostWithModelNew(ConnectionRequestModel model)
    {
        if (string.IsNullOrEmpty(TextAreaValue))
        {
            TextAreaValidationErrorMessage = "Enter a reason for the connection request";
            return Task.FromResult<IActionResult>(Page());
        }

        if (TextAreaValue.Length > 500)
        {
            TextAreaValidationErrorMessage = "Reason for the connection request must be 500 characters or less";
            return Task.FromResult<IActionResult>(Page());
        }

        model.Reason = TextAreaValue;

        return Task.FromResult(NextPage());
    }
}
