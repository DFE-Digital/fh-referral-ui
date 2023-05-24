using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Helper;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class SupportDetailsModel : ProfessionalReferralModel, ISingleTextboxPageModel
{
    public string HeadingText { get; set; } = "Who should the service contact in the family?";
    public string? HintText { get; set; } = "This must be a person aged 16 or over.";
    public string TextBoxLabel { get; set; } = "Full name";
    public string ErrorText { get; set; } = "Enter a full name";

    [Required]
    [BindProperty]
    public string? TextBoxValue { get; set; }

    public SupportDetailsModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
        : base(connectionRequestDistributedCache, ConnectJourneyPage.SupportDetails)
    {
    }

    protected override async Task<IActionResult> OnSafeGetAsync()
    {
        if (Errors != null)
        {
            //todo: use Errors directly
            ValidationValid = false;
        }
        else
        {
            var model = await ConnectionRequestCache.GetAsync(ProfessionalUser.Email);

            if (!string.IsNullOrEmpty(model?.FamilyContactFullName))
            {
                TextBoxValue = model.FamilyContactFullName;
            }
        }

        return Page();
    }

    protected override async Task<IActionResult> OnSafePostAsync()
    {
        if (!ModelState.IsValid)
        {
            return RedirectToSelf(ProfessionalReferralError.SingleTextboxPage_Invalid);
        }

        if (TextBoxValue!.Length > 255)
        {
            TextBoxValue = TextBoxValue.Truncate(252);
        }

        var model = await ConnectionRequestCache.GetAsync(ProfessionalUser.Email)
                    ?? new ConnectionRequestModel
                    {
                        ServiceId = ServiceId
                    };

        model.FamilyContactFullName = TextBoxValue;
        await ConnectionRequestCache.SetAsync(ProfessionalUser.Email, model);

        return NextPage();
    }
}
