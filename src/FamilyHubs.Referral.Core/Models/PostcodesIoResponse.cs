using System.Text.Json.Serialization;
// ReSharper disable UnusedMember.Global
// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo

namespace FamilyHubs.Referral.Core.Models;
public class PostcodesIoResponse
{
    [JsonPropertyName("error")]
    public int Error { get; set; }

    [JsonPropertyName("result")]
    public PostcodeInfo Result { get; set; } = default!;
}

public class PostcodeInfo
{
    /// <summary>
    /// Searched postcode in canonical format. 2, 3 or 4-character outward code, single space and 3-character inward code.
    /// </summary>
    [JsonPropertyName("postcode")]
    public string Postcode { get; set; } = default!;

    public string AdminArea => string.Equals(Codes.AdminCounty, "E99999999", StringComparison.InvariantCultureIgnoreCase) ? Codes.AdminDistrict : Codes.AdminCounty;

    /// <summary>
    /// The WGS84 latitude given the postcode's national grid reference. May be null if geolocation not available.
    /// </summary>
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    /// <summary>
    /// The WGS84 longitude given the postcode's national grid reference. May be null if geolocation not available.
    /// </summary>
    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    /// <summary>
    /// The outward code is the part of the postcode before the single space in the middle. It is between two and four characters long. A few outward codes are non-geographic, not divulging where mail is to be sent. Examples of outward codes include "L1", "W1A", "RH1", "RH10" or "SE1P".
    /// </summary>
    [JsonPropertyName("outcode")]
    public string? OutCode { get; set; }

    [JsonPropertyName("codes")]
    public Codes Codes { get; set; } = default!;
}

public sealed class Codes
{
    /// <summary>
    /// The current district/unitary authority to which the postcode has been assigned. (ID version)
    /// </summary>
    [JsonPropertyName("admin_district")]
    public string AdminDistrict { get; set; } = default!;

    [JsonPropertyName("admin_county")]
    public string AdminCounty { get; set; } = default!;
}