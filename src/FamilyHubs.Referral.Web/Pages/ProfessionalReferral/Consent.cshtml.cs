using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.SharedKernel.Razor.FullPages.Radios;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class ConsentModel : ProfessionalReferralCacheModel, IRadiosPageModel
{
    public string? DescriptionPartial { get; } = "/Pages/ProfessionalReferral/ConsentContent.cshtml";
    public string? Legend { get; } = "Do you have permission to share their details?";

    public IEnumerable<IRadio> Radios { get; } = new[]
    {
        new Radio("Yes",  bool.TrueString),
        new Radio("No", bool.FalseString)
    };

    [BindProperty]
    public string? SelectedValue { get; set; }
    public bool AreRadiosInline { get; } = true;

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
