using FamilyHubs.Referral.Core.ValidationAttributes;

namespace FamilyHubs.ReferralUi.UnitTests.Core.ValidationAttributes;

public class UKGdsPostcodeAttributeTests
{
    public UKGdsPostcodeAttribute UKGdsPostcodeAttribute { get; set; }

    public UKGdsPostcodeAttributeTests()
    {
        UKGdsPostcodeAttribute = new UKGdsPostcodeAttribute();
    }

    [Theory]
    [InlineData(true, "SW1A 1AA")]
    public void IsValid_WhenPostcodeIsValid_ReturnsTrue(bool expectedIsValid, string postcode)
    {
        // Act
        var result = UKGdsPostcodeAttribute.IsValid(postcode);

        Assert.True(expectedIsValid);
    }
}
