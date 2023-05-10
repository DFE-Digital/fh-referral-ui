using System.ComponentModel.DataAnnotations;
using FamilyHubs.Referral.Core.DistributedCache;
using FamilyHubs.Referral.Core.Models;
using FamilyHubs.Referral.Core.ValidationAttributes;
using FamilyHubs.Referral.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FamilyHubs.Referral.Web.Pages.ProfessionalReferral;

public class LetterModel : ProfessionalReferralSessionModel
{
    //todo: consistency with nullable
    [BindProperty]
    [Required(ErrorMessage = "You must enter an address.")]
    public string? AddressLine1 { get; set; } = "";

    [BindProperty]
    public string? AddressLine2 { get; set; } = "";

    [BindProperty]
    [Required(ErrorMessage = "You must enter a town or city.")]
    public string? TownOrCity { get; set; } = "";

    [BindProperty]
    public string? County { get; set; } = "";

    [BindProperty]
    [Required(ErrorMessage = "You must enter a postcode.")]
    [UKGdsPostcode]
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
        model.Postcode = UKGdsPostcodeAttribute.SanitisePostcode(Postcode!);

        return NextPage(ContactMethod.Letter, model.ContactMethodsSelected);
    }

    private void SetPageProperties(ConnectionRequestModel model)
    {
        HeadingText = $"What is the address for {model.FamilyContactFullName}?";
        BackUrl = PreviousPage(ContactMethod.Letter, model.ContactMethodsSelected);
    }
}