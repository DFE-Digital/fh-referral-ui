using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class LetterModel : ProfessionalReferralSessionModel
{
    //todo: consistency with nullable
    [BindProperty]
    [Required]
    public string AddressLine1 { get; set; } = "";
    [BindProperty]
    public string AddressLine2 { get; set; } = "";
    [BindProperty]
    [Required]
    public string TownOrCity { get; set; } = "";
    [BindProperty]
    public string County { get; set; } = "";
    [BindProperty]
    [Required]
    public string Postcode { get; set; } = "";

    public string HeadingText { get; set; } = "";
    public string? BackUrl { get; set; }

    public LetterModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        SetPageProperties(model);
    }

    protected override string? OnPostWithModel(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            ValidationValid = false;
            SetPageProperties(model);
            return null;
        }

        return NextPage(ContactMethod.Letter, model.ContactMethodsSelected);
    }

    private void SetPageProperties(ConnectionRequestModel model)
    {
        HeadingText = $"What is the address for {model.FamilyContactFullName}?";
        BackUrl = PreviousPage(ContactMethod.Letter, model.ContactMethodsSelected);
    }
}