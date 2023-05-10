using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FamilyHubs.Referral.Core.ValidationAttributes;

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
    private static readonly Regex _simpleValidUkPostcodeRegex = new(
        @"^\s*[a-z]{1,2}\d[a-z\d]?\s*\d[a-z]{2}\s*$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // GDS recommends that you allow postcodes, with...
    // 'punctuation like hyphens, brackets, dashes and full stops'
    // I personally think it's a bad idea, as e.g. someone might have accidentally pressed '.' instead of 'l' and we wouldn't catch that.
    // I've also never seen a postcode containing punctuation or a postcode validation that allows it, but hey ho.
    private static readonly Regex GdsAllowableChars = new(
        @"[-\(\)\.\[\]]+", RegexOptions.Compiled);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value != null)
        {
            string postcode = (string)value;

            postcode = GdsAllowableChars.Replace(postcode, "");

            if (!_simpleValidUkPostcodeRegex.IsMatch(postcode))
            {
                return new ValidationResult("Enter a real postcode.");
            }
        }

        return ValidationResult.Success;
    }

    public static string SanitisedPostcode(string postcode)
    {
        return GdsAllowableChars.Replace(postcode.Trim().ToUpper(), "");
    }
}
