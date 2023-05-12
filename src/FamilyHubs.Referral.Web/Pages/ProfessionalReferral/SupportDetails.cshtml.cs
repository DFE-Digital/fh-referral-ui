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
    private readonly IConnectionRequestDistributedCache _connectionRequestDistributedCache;

    public string HeadingText { get; set; } = "Who should the service contact in the family?";
    public string? HintText { get; set; } = "This must be a person aged 16 or over.";
    public string TextBoxLabel { get; set; } = "Full name";
    public string ErrorText { get; set; } = "Enter a full name";
    public bool ValidationValid { get; set; } = true;

    [Required]
    [BindProperty]
    public string? TextBoxValue { get; set; }

    public SupportDetailsModel(IConnectionRequestDistributedCache connectionRequestDistributedCache)
        : base(ConnectJourneyPage.SupportDetails)
    {
        _connectionRequestDistributedCache = connectionRequestDistributedCache;
    }

    protected override async Task<IActionResult> OnSafeGetAsync()
    {
        //todo:
        //Fixes Session Changing between requests 
        HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });

        var model = await _connectionRequestDistributedCache.GetAsync();

        if (!string.IsNullOrEmpty(model?.FamilyContactFullName))
        {
            TextBoxValue = model.FamilyContactFullName;
        }

        return Page();
    }

    protected override async Task<IActionResult> OnSafePostAsync()
    {
        if (!ModelState.IsValid)
        {
            ValidationValid = false;
            return Page();
        }

        if (TextBoxValue!.Length > 255)
        {
            TextBoxValue = TextBoxValue.Truncate(252);
        }

        var model = await _connectionRequestDistributedCache.GetAsync()
                    ?? new ConnectionRequestModel
                    {
                        ServiceId = ServiceId
                    };

        model.FamilyContactFullName = TextBoxValue;
        await _connectionRequestDistributedCache.SetAsync(model);

        return NextPage("WhySupport");
    }
}
