using System.ComponentModel.DataAnnotations;
using PhoneNumbers;

namespace FamilyHubs.Referral.Core.ValidationAttributes;

/// <summary>
/// Validates a UK telephone number.
/// 
/// GDS advises that we allow different formats:
/// Let users enter telephone numbers in whatever format is familiar to them. Allow for additional spaces, hyphens, dashes and brackets, and be able to accommodate country and area codes.
/// </summary>
public class UkGdsTelephoneNumberAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        string phoneNumberString = (string)value;
        return IsValid(phoneNumberString);
    }

    public static ValidationResult IsValid(string phoneNumber)
    {
        bool isValid = false;

        PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
        try
        {
            // throws if not a possible number somewhere in the world
            var parsedPhoneNumber = phoneNumberUtil.Parse(phoneNumber, "GB");
            // does more in depth validation, including if it's a valid UK number
            isValid = phoneNumberUtil.IsValidNumber(parsedPhoneNumber)
                      && phoneNumberUtil.GetRegionCodeForNumber(parsedPhoneNumber) == "GB"
                      // libphonenumber allows some characters that we don't want to allow
                      && !phoneNumber.Intersect("!\"£$%^&*={}'@~\\|?/").Any();
        }
        catch (NumberParseException)
        {
            // PhoneNumberUtil.Parse calls IsViablePhoneNumber(), which throws a NumberParseException if the number isn't a viable telephone number somewhere in the world
        }

        if (!isValid)
        {
            return new ValidationResult("Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192");
        }

        return ValidationResult.Success!;
    }
}