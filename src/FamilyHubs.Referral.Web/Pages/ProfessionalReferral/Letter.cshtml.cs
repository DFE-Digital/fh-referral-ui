using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.ValidationAttributes;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class LetterModel : ProfessionalReferralCacheModel
{
    //todo: consistency with nullable
    [BindProperty]
    [Required(ErrorMessage = "You must enter an address")]
    public string? AddressLine1 { get; set; } = "";

    [BindProperty]
    public string? AddressLine2 { get; set; } = "";

    [BindProperty]
    [Required(ErrorMessage = "You must enter a town or city")]
    public string? TownOrCity { get; set; } = "";

    [BindProperty]
    public string? County { get; set; } = "";

    [BindProperty]
    [Required(ErrorMessage = "You must enter a postcode")]
    [UkGdsPostcode]
    public string? Postcode { get; set; } = "";

    public string HeadingText { get; set; } = "";

    public LetterError[]? LetterErrors { get; set; }

    public LetterModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(ConnectJourneyPage.Letter, connectionRequestCache)
    {
    }

    protected override void OnGetWithModel(ConnectionRequestModel model)
    {
        AddressLine1 = model.AddressLine1;
        AddressLine2 = model.AddressLine2;
        TownOrCity = model.TownOrCity;
        County = model.County;
        Postcode = model.Postcode;

        SetPageProperties(model);
    }

    //todo: make this generic
    public record LetterError(string Property, string ErrorMessage);

    // the ordering of errors in the ModelState is not guaranteed
    private IEnumerable<LetterError> GetErrors(params string[] propertyNames)
    {
        return propertyNames.Select(p => (propertyName: p, entry: ModelState[p]))
            .Where(t => t.entry!.ValidationState == ModelValidationState.Invalid)
            .Select(t => new LetterError(t.propertyName, t.entry!.Errors[0].ErrorMessage));
    }

    protected override IActionResult OnPostWithModel(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            LetterErrors = GetErrors(nameof(AddressLine1), nameof(TownOrCity), nameof(Postcode)).ToArray();
            HasErrors = true;
            SetPageProperties(model);
            return Page();
        }

        model.AddressLine1 = AddressLine1;
        model.AddressLine2 = AddressLine2;
        model.TownOrCity = TownOrCity;
        model.County = County;
        model.Postcode = UkGdsPostcodeAttribute.SanitisePostcode(Postcode!);

        return NextPage(ConnectContactDetailsJourneyPage.Letter, model.ContactMethodsSelected);
    }

    private void SetPageProperties(ConnectionRequestModel model)
    {
        HeadingText = $"What is the address for {model.FamilyContactFullName}?";
        BackUrl = GenerateBackUrl(ConnectContactDetailsJourneyPage.Letter, model.ContactMethodsSelected);
    }
}