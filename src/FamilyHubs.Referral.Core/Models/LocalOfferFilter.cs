namespace FamilyHubs.Referral.Core.Models;

public record LocalOfferFilter
{
    public string ServiceType { get; set; } = default!;
    public string Status { get; set; } = default!;
    public bool? AllChildrenYoungPeople { get; init; }
    public int? GivenAge { get; init; }
    public string? DistrictCode { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public double? Proximity { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public string? Text { get; set; } = default!;
    public string? ServiceDeliveries { get; init; }
    public bool? IsPaidFor { get; init; }
    public string? TaxonomyIds { get; init; }
    public string? Languages { get; init; }
    public bool? CanFamilyChooseLocation { get; init; }
}
