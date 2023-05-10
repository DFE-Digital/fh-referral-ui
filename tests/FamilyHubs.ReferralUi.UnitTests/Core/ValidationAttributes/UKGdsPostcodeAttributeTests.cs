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
    [InlineData(true, "[SW1A 1AA]")]
    [InlineData(true, "SW1A-1AA")]
    [InlineData(true, "S.W.1.A.1.A.A")]
    [InlineData(true, " S.W.1.A.1.A.A")]
    [InlineData(true, " []-------[]()(][)S].W.-1.A  [.1-.A.[-A          ")] // really GDS!?
    [InlineData(true, "B1 1AA")]
    [InlineData(true, "CF14 1AA")]
    [InlineData(true, "sw1A  1aa")]
    [InlineData(true, "  sW1A    1aa  ")]   // tabs
    [InlineData(true, " Sw1A    1Aa   ")]   // tabs
    [InlineData(true, "(sw1A 1aA)")]
    [InlineData(true, "sw1A-1sw")]
    [InlineData(true, "s.w.1.A.1.a.a")]
    [InlineData(true, "b1 1aa")]
    [InlineData(true, "cf14 1aa")]
    [InlineData(false, "B 11AA")]
    [InlineData(false, "NOTAPOSTCODE")]
    [InlineData(false, "B11A 1")]
    [InlineData(false, "123")]
    [InlineData(false, "       ")]
    public void IsValid_WhenPostcodeIsTested_ReturnsWhetherIsValid(bool expectedIsValid, string? postcode)
    {
        // Act
        var result = UKGdsPostcodeAttribute.IsValid(postcode);

        if (expectedIsValid)
            Assert.True(result);
        else
            Assert.False(result);
    }

    [Theory]
    [InlineData("SW1A 1AA", "SW1A 1AA")]
    [InlineData("SW1A 1AA", " SW1A 1AA")]
    [InlineData("SW1A 1AA", "SW1A 1AA ")]
    [InlineData("SW1A 1AA", "SW1A  1AA")]
    [InlineData("SW1A 1AA", "  SW1A    1AA  ")]   // tabs
    [InlineData("SW1A 1AA", " SW1A    1AA   ")]   // tabs
    [InlineData("SW1A 1AA", "[SW1A 1AA]")]
    [InlineData("SW1A1AA", "SW1A-1AA")]
    [InlineData("SW1A1AA", "S.W.1.A.1.A.A")]
    [InlineData("SW1A1AA", " S.W.1.A.1.A.A")]
    [InlineData("SW1A 1AA", " []-------[]()(][)S].W.-1.A  [.1-.A.[-A          ")] // really GDS!?
    [InlineData("B1 1AA", "B1 1AA")]
    [InlineData("CF14 1AA", "CF14 1AA")]
    [InlineData("SW1A 1AA", "sw1A  1aa")]
    [InlineData("SW1A 1AA", "  sW1A    1aa  ")]   // tabs
    [InlineData("SW1A 1AA", " Sw1A    1Aa   ")]   // tabs
    [InlineData("SW1A 1AA", "(sw1A 1aA)")]
    [InlineData("SW1A1SW", "sw1A-1sw")]
    [InlineData("SW1A1AA", "s.w.1.A.1.a.a")]
    [InlineData("B1 1AA", "b1 1aa")]
    [InlineData("CF14 1AA", "cf14 1aa")]
    public void SanitisePostcode_WhenPostcodeIsSupplied_ReturnsSanitisedPostcode(string expectedPostcode, string postcode)
    {
        // Act
        string result = UKGdsPostcodeAttribute.SanitisePostcode(postcode);

        Assert.Equal(expectedPostcode, result);
    }
}
