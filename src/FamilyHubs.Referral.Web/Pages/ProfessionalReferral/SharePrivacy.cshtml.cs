using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SharePrivacyModel : ProfessionalReferralCacheModel
{
    //todo: change consent too
    [BindProperty]
    public bool? SharedPrivacy { get; set; }

    public SharePrivacyModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
        : base(ConnectJourneyPage.SharePrivacy, connectionRequestDistributedCache)
    {
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        //when is IsValid false?
        if (!ModelState.IsValid || SharedPrivacy == null)
        {
            return RedirectToSelf(null, ErrorId.SharePrivacy_NoSelection);
        }

        if (SharedPrivacy.Value)
        {
            return NextPage();
        }

        return RedirectToProfessionalReferralPage("SharePrivacyShutter");
    }
}