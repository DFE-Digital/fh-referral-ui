using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.SharedKernel.Razor.FullPages.Radios;
using FamilyHubs.SharedKernel.Razor.FullPages.Radios.Common;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ConsentModel : ProfessionalReferralCacheModel, IRadiosPageModel
{
    public string DescriptionPartial => "/Pages/ProfessionalReferral/ConsentContent.cshtml";
    public string Legend => "Do you have permission to share their details?";

    public IEnumerable<IRadio> Radios => CommonRadios.YesNo;

    [BindProperty]
    public string? SelectedValue { get; set; }
    public bool AreRadiosInline => true;

    public bool? Consent
    {
        get => bool.TryParse(SelectedValue, out var result) ? result : null;
        set => SelectedValue = value.ToString();
    }

    public ConsentModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
        : base(ConnectJourneyPage.Consent, connectionRequestDistributedCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        if (model.ConsentGiven)
        {
            Consent = true;
        }
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid || Consent == null)
        {
            return RedirectToSelf(null, ErrorId.Consent_NoConsentSelected);
        }

        if (Consent.Value)
        {
            model.ConsentGiven = true;
            return NextPage();
        }

        return RedirectToProfessionalReferralPage("ConsentShutter");
    }
}
