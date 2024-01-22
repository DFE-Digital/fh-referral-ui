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

        if (Errors.HasTriggeredError((int)ErrorId.WhySupport_TooLong))
        {
            TextAreaValue = model.ErrorState!.InvalidUserInput![0];
        }
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        if (string.IsNullOrEmpty(TextAreaValue))
        {
            return RedirectToSelf(null, ErrorId.WhySupport_NothingEntered);
        }

        // workaround the front end counting line endings as 1 chars (\n) as per HTML spec,
        // and the http transport/.net/windows using 2 chars for line ends (\r\n)
        if (TextAreaValue.Replace("\r", "").Length > 500)
        {
            return RedirectToSelf(TextAreaValue, ErrorId.WhySupport_TooLong);
        }

        model.Reason = TextAreaValue;

        return NextPage();
    }
}
