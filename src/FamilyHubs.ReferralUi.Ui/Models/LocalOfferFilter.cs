namespace FamilyHubs.ReferralUi.Ui.Models;

public record LocalOfferFilter
{
    public string ServiceType { get; init; }
    public string Status { get; set; }
    public int? MinimumAge { get; init; }
    public int? MaximumAge { get; init; }
    public int? GivenAge { get; init; }
    public string? DistrictCode { get; init; }
    public double? Latitude { get; init; }
    public double? Longtitude { get; init; }
    public double? Proximity { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public string Text { get; init; }
    public string? ServiceDeliveries { get; init; }
    public bool? IsPaidFor { get; init; }
    public string? TaxonmyIds { get; init; }
    public string? Languages { get; init; }
    public bool? CanFamilyChooseLocation { get; init; }
}
