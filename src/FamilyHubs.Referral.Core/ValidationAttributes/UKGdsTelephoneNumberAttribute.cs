using System.ComponentModel.DataAnnotations;
using PhoneNumbers;

namespace FamilyHubs.Referral.Core.ValidationAttributes;

/// <summary>
/// GDS advice:
/// Allow different formats
/// -----------------------
/// Let users enter telephone numbers in whatever format is familiar to them.Allow for additional spaces, hyphens, dashes and brackets, and be able to accommodate country and area codes.
/// Validate telephone numbers
/// --------------------------
/// You should validate telephone numbers so you can let users know if they have entered one incorrectly.Google’s libphonenumber library can validate telephone numbers from most countries.
///
/// Notes
/// -----
/// This only accepts UK telephone numbers.
/// Do we allow extensions? e.g. 020 7946 0000 ext 1234 or 020 7946 0000 x1234 or 020 7946 0000 # 1234 etc.
/// As GDS recommends we validate numbers, we don't allow these examples:
/// 123 456 7890 until 6pm, then 098 765 4321  
/// 123 456 7890 or try my mobile on 098 765 4321
/// 123456 in the week or 567890 at weekends
/// </summary>
public class UKGdsTelephoneNumberAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value != null)
        {
            string phoneNumberString = (string)value;
            bool isValid = false;

            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
            try
            {
                // throws if not a possible number somewhere in the world
                var phoneNumber = phoneNumberUtil.Parse(phoneNumberString, "GB");
                // does more in depth validation, including if it's a valid UK number
                isValid = phoneNumberUtil.IsValidNumber(phoneNumber);
            }
            catch (NumberParseException ex)
            {
                // PhoneNumberUtil.Parse calls IsViablePhoneNumber(), which throws a NumberParseException if the number isn't a viable telephone number somewhere in the world
            }

            if (!isValid)
            {
                return new ValidationResult("Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192");
            }
        }

        return ValidationResult.Success;
    }
}