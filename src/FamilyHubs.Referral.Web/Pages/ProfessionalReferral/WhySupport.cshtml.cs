using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class WhySupportModel : ProfessionalReferralSessionModel, ITellTheServicePageModel
{
    public string DescriptionPartial => "/Pages/ProfessionalReferral/WhySupportContent.cshtml";
    public string? TextAreaValidationErrorMessage { get; set; }

    [BindProperty]
    public string? TextAreaValue { get; set; }

    public WhySupportModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        if (!string.IsNullOrEmpty(model.Reason))
            TextAreaValue = model.Reason;
    }

    protected override string? OnPostWithModel(ConnectionRequestModel model)
    {
        if (string.IsNullOrEmpty(TextAreaValue))
        {
            TextAreaValidationErrorMessage = "Enter details about the family";
            return null;
        }

        if (TextAreaValue.Length > 500)
        {
            TextAreaValidationErrorMessage = "Reason for the connection request must be 500 characters or less";
            return null;
        }

        model.Reason = TextAreaValue;

        return "ContactDetails";
    }
}
