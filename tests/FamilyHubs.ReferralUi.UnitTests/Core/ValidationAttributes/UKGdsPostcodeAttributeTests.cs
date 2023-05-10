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
    [InlineData(true, " SW1A 1AA")]
    [InlineData(true, "SW1A 1AA ")]
    [InlineData(true, "SW1A  1AA")]
    [InlineData(true, "  SW1A    1AA  ")]   // tabs
    [InlineData(true, " SW1A    1AA   ")]   // tabs
    [InlineData(true, "SW1A  1AA")]
    [InlineData(true, "[SW1A 1AA]")]
    [InlineData(true, "SW1A-1AA")]
    [InlineData(true, "SW1A-1AA")]
    [InlineData(true, "S.W.1.A.1.A.A")]
    [InlineData(true, " S.W.1.A.1.A.A")]
    [InlineData(true, " []-------[]()(][)S].W.-1.A  [.1-.A.[-A          ")] // really GDS!?
    [InlineData(true, "B1 1AA")]
    [InlineData(true, "CF14 1AA")]
    [InlineData(true, "sw1A  1aa")]
    [InlineData(true, "  sW1A    1aa  ")]   // tabs
    [InlineData(true, " Sw1A    1Aa   ")]   // tabs
    [InlineData(true, "sw1A  1aa")]
    [InlineData(true, "(sw1A 1aA)")]
    [InlineData(true, "sw1A-1sw")]
    [InlineData(true, "sw1A-1sw")]
    [InlineData(true, "s.w.1.A.1.a.a")]
    [InlineData(true, "B1 1AA")]
    [InlineData(true, "CF14 1AA")]
    [InlineData(false, "B 11AA")]
    [InlineData(false, "NOTAPOSTCODE")]
    [InlineData(false, "B11A 1")]
    [InlineData(false, "123")]
    [InlineData(false, "       ")]
    public void IsValid_WhenPostcodeIsTested_ReturnsWhetherIsValid(bool expectedIsValid, string postcode)
    {
        // Act
        var result = UKGdsPostcodeAttribute.IsValid(postcode);

        if (expectedIsValid)
            Assert.True(result);
        else
            Assert.False(result);
    }
}
