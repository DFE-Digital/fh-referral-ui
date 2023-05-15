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
    [InlineData(true, "(01788) 861 384")]
    [InlineData(true, "+44 808 157 0192")]
    [InlineData(true, "01656861385")]
    [InlineData(true, "+44-808-157-0192")]
    [InlineData(true, "+44-(808)-157-0192")]
    [InlineData(true, "+44-808-(157)-0192")]
    [InlineData(true, "+44-808-157-(0192)")]
    [InlineData(true, "01656 861 384")]
    [InlineData(true, "07462 602236")]

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