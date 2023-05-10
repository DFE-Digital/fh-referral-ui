using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class WhySupportModel : ProfessionalReferralSessionModel, ITellTheServicePageModel
{
    public string DescriptionPartial => "/Pages/ProfessionalReferral/WhySupportContent.cshtml";

    [BindProperty]
    public string? TextAreaValue { get; set; }

    public TextAreaValidation TextAreaValidation { get; set; } = TextAreaValidation.Valid;

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
            TextAreaValidation = TextAreaValidation.Empty;
            return null;
        }

        if (TextAreaValue.Length > 500)
        {
            TextAreaValidation = TextAreaValidation.TooLong;
            return null;
        }

        model.Reason = TextAreaValue;

        return "ContactDetails";
    }
}
