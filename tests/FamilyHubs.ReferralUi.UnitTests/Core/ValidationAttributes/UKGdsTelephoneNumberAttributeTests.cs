using FamilyHubs.Referral.Core.ValidationAttributes;
using FluentAssertions;

namespace FamilyHubs.ReferralUi.UnitTests.Core.ValidationAttributes;

public class UKGdsTelephoneNumberAttributeTests
{
    public UKGdsTelephoneNumberAttribute UKGdsTelephoneNumberAttribute { get; set; }

    public UKGdsTelephoneNumberAttributeTests()
    {
        UKGdsTelephoneNumberAttribute = new UKGdsTelephoneNumberAttribute();
    }

    [Theory]
    // GDS says allow country code
    [InlineData(true, "+44 808 157 0192")]

    // GDS says allow country code, but we're only allowing UK numbers
    [InlineData(false, "+1 212-555-1234")]
    [InlineData(false, "+61 2 5555 1234")]
    [InlineData(false, "+86 10 5555 1234")]

    // no spaces
    [InlineData(true, "01656861385")]
    [InlineData(true, "07462602236")]

    // example vanity numbers (not really a thing in the UK), but libphonenumber allows it
    [InlineData(true, "0746260abcd")]

    // if someone mistypes and replaces a number with a letter, it's taken as invalid
    [InlineData(false, "0746260a123")]

    // an extra alpha or two in the middle of a valid number are ignored
    [InlineData(true, "0746260a1234")]
    [InlineData(true, "0746260A1234")]
    [InlineData(true, "0746260i1234")]
    [InlineData(true, "0746260ig1234")]

    // any more than two are classed as invalid
    [InlineData(false, "0746260ign1234")]
    [InlineData(false, "0746260igno1234")]
    [InlineData(false, "0746260ignor1234")]
    [InlineData(false, "0746260ignore1234")]
    [InlineData(false, "0746260ignored1234")]

    // some alphas can't be part of vanity numbers
    [InlineData(false, "0746260q234")]
    [InlineData(false, "0746260Q234")]
    [InlineData(false, "0746260z234")]
    [InlineData(false, "0746260Z234")]

    // GDS says allow additional spaces, hyphens, dashes and brackets

    // spaces
    [InlineData(true, "01656 861 384")]
    [InlineData(true, "07462 602236")]
    [InlineData(true, " 0 7 4 6 2 6 0 2 2 3 6 ")]

    // hyphen
    [InlineData(true, "+44-808-157-0192")]
    [InlineData(true, "-+44-808-157-0192-")]
    // em dash
    [InlineData(true, "+44—808—157—0193")]
    // en dash
    [InlineData(true, "+44–808–157–0194")]

    // brackets
    [InlineData(true, "(01788)861384")]
    [InlineData(true, "01788(861)384")]
    [InlineData(true, "[01788]861384")]

    // mix
    [InlineData(true, "(01788) 861 384")]
    [InlineData(true, "01788 (861) 384")]
    [InlineData(true, "01788 861 (384)")]
    [InlineData(true, "(01788) (861) (384)")]
    [InlineData(true, "+44-[808]-157-0192")]
    [InlineData(true, "+44-(808)-157-0192")]
    [InlineData(true, "+44-808-(157)-0192")]
    [InlineData(true, "+44-808-157-(0192)")]
    [InlineData(true, "+]4((4)]-808-157-(0192)")]

    // non-numbers
    [InlineData(false, "my number")]
    [InlineData(false, "MYNUMBER")]
    [InlineData(false, "x")]
    [InlineData(false, "X")]

    // invalid punctuation
    [InlineData(false, "+<44-808-157-0192>")]
    [InlineData(false, "+44!808!157!0192>")]
    // libphonenumber for some reason allows this. has it got a borked regex?
    [InlineData(false, "+448081570192$")]
    [InlineData(false, "+44808157019$2")]
    [InlineData(false, "^+448081570192")]
    [InlineData(false, "*+448081570192")]
    [InlineData(false, "£+448081570192")]

    // some examples we may see (these sort of things can be entered into the 'how to engage with the family' field) later on)
    [InlineData(false, "01656861389 before the 12th May, 01722474474 after")]
    [InlineData(false, "On weekdays 01656861389 and on the weekend try 01722474474 or 0797627272")]
    [InlineData(false, "Either 01656861389 or 01656861389")]
    [InlineData(false, "try hostel:01656861389, Friends house:01656861389")]

    // UK-wide : OK, contact might be giving a work number (probably with an extension)
    [InlineData(true, "03069 990000")]
    // Premium rate : we probably don't want to allow these, or at least warn the user
    [InlineData(true, "0909 8790000")]
    // Freephone
    [InlineData(true, "08081 570000")]

    // valid extension formats
    [InlineData(true, "01656 861 384 ext. 12345")]
    [InlineData(true, "01656 861 384#1")]
    [InlineData(true, "01656 861 384 x 123")]
    [InlineData(true, "01656 861 384 extension 12345678")]

    // invalid extension formats
    [InlineData(false, "01656 861 384 e 12345678")]
    [InlineData(false, "01656 861 384 bob 12345678")]

    // incorrect number of digits
    [InlineData(false, "0")]
    [InlineData(false, "01")]
    [InlineData(false, "016")]
    [InlineData(false, "0165")]
    [InlineData(false, "01656")]
    [InlineData(false, "01656 8")]
    [InlineData(false, "01656 86")]
    [InlineData(false, "01656 861")]
    [InlineData(false, "01656 861 3")]
    [InlineData(false, "01656 861 38")]
    [InlineData(false, "01656 861 3845")]
    [InlineData(false, "01656 861 38456")]

    // this is a designated example number, and is in a valid format, but it's not a real geographic area code
    [InlineData(false, "01632 960 001")]
    // this is a designated example mobile number, and is in  a valid format, but it's not a real mobile area code
    [InlineData(false, "07700 900 982")]
    public void IsValid_ShouldReturnIfNumberIsValid(bool expected, string number)
    {
        UKGdsTelephoneNumberAttribute.IsValid(number).Should().Be(expected);
    }
}