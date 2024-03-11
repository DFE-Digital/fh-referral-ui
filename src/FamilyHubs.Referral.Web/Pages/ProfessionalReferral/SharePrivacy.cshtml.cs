using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.SharedKernel.Razor.FullPages.Radios;
using FamilyHubs.SharedKernel.Razor.FullPages.Radios.Common;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SharePrivacyModel : ProfessionalReferralCacheModel, IRadiosPageModel
{
    public string? DescriptionPartial => "/Pages/ProfessionalReferral/SharePrivacyContent.cshtml";
    public string? Legend => "Have you shared our privacy statement?";

    public IEnumerable<IRadio> Radios => CommonRadios.YesNo;

    [BindProperty]
    public string? SelectedValue { get; set; }
    public bool AreRadiosInline => true;

    public bool? SharedPrivacy
    {
        get => bool.TryParse(SelectedValue, out var result) ? result : null;
        set => SelectedValue = value.ToString();
    }
    public SharePrivacyModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
        : base(ConnectJourneyPage.SharePrivacy, connectionRequestDistributedCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        if (model.PrivacyShared)
        {
            SharedPrivacy = true;
        }
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
            model.PrivacyShared = true;
            return NextPage();
        }

        return RedirectToProfessionalReferralPage("SharePrivacyShutter");
    }
}