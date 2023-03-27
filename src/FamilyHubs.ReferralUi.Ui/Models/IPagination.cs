
namespace FamilyHubs.ReferralUi.Ui.Models;

public interface IPagination
{
    IEnumerable<PaginationItem> PaginationItems { get; }
    bool Show { get; }
    int? TotalPages { get; }
    int? CurrentPage { get; }
    int? PreviousPage { get; }
    int? NextPage { get; }
}