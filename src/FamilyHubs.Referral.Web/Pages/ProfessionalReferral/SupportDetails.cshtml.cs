using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Helper;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SupportDetailsModel : ProfessionalReferralCacheModel, ISingleTextboxPageModel
{
    public string HeadingText { get; set; } = "Who should the service contact in the family?";
    public string? HintText { get; set; } = "This must be a person aged 16 or over.";
    public string TextBoxLabel { get; set; } = "Full name";
    public string ErrorText { get; set; } = "Enter a full name";

    [Required]
    [BindProperty]
    public string? TextBoxValue { get; set; }

    public SupportDetailsModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
        : base(ConnectJourneyPage.SupportDetails, connectionRequestDistributedCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        if (ValidationValid)
        {
            TextBoxValue = model.FamilyContactFullName;
        }
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToSelf(null, ProfessionalReferralError.SupportDetails_Invalid);
        }

        if (TextBoxValue!.Length > 255)
        {
            TextBoxValue = TextBoxValue.Truncate(252);
        }

        model.FamilyContactFullName = TextBoxValue;

        return NextPage();
    }
}
