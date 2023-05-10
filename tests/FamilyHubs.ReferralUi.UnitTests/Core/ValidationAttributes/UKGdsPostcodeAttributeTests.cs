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
    [InlineData(true, "[SW1A 1AA]")]
    [InlineData(true, "SW1A-1AA")]
    [InlineData(true, "SW1A-1AA")]
    [InlineData(true, "S.W.1.A.1.A.A")]
    public void IsValid_WhenPostcodeIsTested_ReturnsWhetherIsValid(bool expectedIsValid, string postcode)
    {
        // Act
        var result = UKGdsPostcodeAttribute.IsValid(postcode);

        Assert.True(expectedIsValid);
    }
}
