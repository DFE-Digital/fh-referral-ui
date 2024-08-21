using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Helper;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using FamilyHubs.SharedKernel.Razor.FullPages.SingleTextbox;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SupportDetailsModel : ProfessionalReferralCacheModel, ISingleTextboxPageModel
{
    public string HeadingText { get; set; } = "Who should this service contact?";
    public string? HintText { get; set; } = "This person must be 16 or over.";
    public string TextBoxLabel { get; set; } = "Full name";
    public int? MaxLength => 255;

    [Required]
    [BindProperty]
    public string? TextBoxValue { get; set; }

    public SupportDetailsModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
        : base(ConnectJourneyPage.SupportDetails, connectionRequestDistributedCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        if (!HasErrors)
        {
            TextBoxValue = model.FamilyContactFullName;
        }
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToSelf(null, ErrorId.SupportDetails_Invalid);
        }

        if (TextBoxValue!.Length > MaxLength)
        {
            TextBoxValue = TextBoxValue.Truncate(MaxLength.Value-3);
        }

        model.FamilyContactFullName = TextBoxValue;

        return NextPage();
    }
}
