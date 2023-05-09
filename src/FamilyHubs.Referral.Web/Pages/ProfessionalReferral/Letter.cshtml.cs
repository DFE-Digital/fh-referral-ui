using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class UKGdsPostcodeAttribute : ValidationAttribute
{
    // This is a simple 'does it look like a postcode' check.
    // We could use a more complex regex to check for valid postcodes,
    // but it's harder to get right and would require regular updating.
    // We could call postcode.io and let it check the postcode,
    // but that would require an API call for every postcode entered (not a biggie),
    // but if postcodes.io is out-of-date, (which it seems to be sometimes),
    // we could stop a valid postcode being entered.
    // see, https://ideal-postcodes.co.uk/guides/postcode-validation

    // allows whitespace at the start, end and in the middle
    private static Regex _simpleValidUkPostcodeRegex = new Regex(
        @"^\s*[a-z]{1,2}\d[a-z\d]?\s*\d[a-z]{2}\s*$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // GDS recommends that you allow postcodes, with...
    // 'punctuation like hyphens, brackets, dashes and full stops'
    // I personally think it's a bad idea, as e.g. someone might have accidentally pressed '.' instead of 'l' and we wouldn't catch that.
    // I've also never seen a postcode containing punctuation or a postcode validation that allows it, but hey ho.
    private static Regex _gdsAllowableChars = new Regex(
        @"[-\(\).]+", RegexOptions.Compiled);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value != null)
        {
            string postcode = ((string)value);

            postcode = _gdsAllowableChars.Replace(postcode, "");

            if (!_simpleValidUkPostcodeRegex.IsMatch(postcode))
            {
                return new ValidationResult("Enter a real postcode.");
            }
        }

        return ValidationResult.Success;
    }
}

public class LetterModel : ProfessionalReferralSessionModel
{
    //todo: consistency with nullable
    [BindProperty]
    [Required(ErrorMessage = "You must enter an address.")]
    [Display(Order = 1)]
    public string? AddressLine1 { get; set; } = "";

    [BindProperty]
    public string? AddressLine2 { get; set; } = "";

    [BindProperty]
    [Required(ErrorMessage = "You must enter a town or city.")]
    [Display(Order = 2)]
    public string? TownOrCity { get; set; } = "";

    [BindProperty]
    public string? County { get; set; } = "";

    [BindProperty]
    [Required(ErrorMessage = "You must enter a postcode.")]
    [UKGdsPostcode]
    [Display(Order = 3)]
    public string? Postcode { get; set; } = "";

    public string HeadingText { get; set; } = "";
    public string? BackUrl { get; set; }

    public Error[]? Errors { get; set; }

    public LetterModel(IConnectionRequestDistributedCache connectionRequestCache)
        : base(connectionRequestCache)
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

    // have partial for summary?
    //todo: have valid bool?
    public record Error(string Property, string ErrorMessage);

    // the ordering of errors in the ModelState is not guaranteed
    private IEnumerable<Error> GetErrors(params string[] propertyNames)
    {
        return propertyNames.Select(p => (propertyName: p, entry: ModelState[p]))
            .Where(t => t.entry!.ValidationState == ModelValidationState.Invalid)
            .Select(t => new Error(t.propertyName, t.entry!.Errors[0].ErrorMessage));
    }

    protected override string? OnPostWithModel(ConnectionRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            Errors = GetErrors(nameof(AddressLine1), nameof(TownOrCity), nameof(Postcode)).ToArray();
            ValidationValid = false;
            SetPageProperties(model);
            return null;
        }

        model.AddressLine1 = AddressLine1;
        model.AddressLine2 = AddressLine2;
        model.TownOrCity = TownOrCity;
        model.County = County;
        model.Postcode = Postcode;

        return NextPage(ContactMethod.Letter, model.ContactMethodsSelected);
    }

    private void SetPageProperties(ConnectionRequestModel model)
    {
        HeadingText = $"What is the address for {model.FamilyContactFullName}?";
        BackUrl = PreviousPage(ContactMethod.Letter, model.ContactMethodsSelected);
    }
}