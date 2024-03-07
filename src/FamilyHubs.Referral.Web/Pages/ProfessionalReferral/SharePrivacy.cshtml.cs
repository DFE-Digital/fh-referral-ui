using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.SharedKernel.Razor.FullPages.Radios;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SharePrivacyModel : ProfessionalReferralCacheModel, IRadiosPageModel
{
    public string? DescriptionPartial { get; } = "/Pages/ProfessionalReferral/SharePrivacyContent.cshtml";
    public string? Legend { get; } = "Have you shared our privacy statement?";

    public IEnumerable<IRadio> Radios { get; } = new[]
    {
        new Radio("Yes",  bool.TrueString),
        new Radio("No", bool.FalseString)
    };

    [BindProperty]
    public string? SelectedValue { get; set; }
    public bool AreRadiosInline { get; } = true;

    public bool? SharedPrivacy
    {
        get => SelectedValue == null ? null : Convert.ToBoolean(SelectedValue);
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