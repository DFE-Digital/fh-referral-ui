namespace FamilyHubs.Referral.Core.Models;

public interface IPagination
{
    IEnumerable<PaginationItem> PaginationItems => Enumerable.Empty<PaginationItem>();
    bool Show => false;
    int? TotalPages { get; }
    int? CurrentPage { get; }
    int? PreviousPage { get; }
    int? NextPage { get; }
}

public class LargeSetPagination : IPagination
{
    public IEnumerable<PaginationItem> PaginationItems { get; }

    public bool Show { get; }
    public int? TotalPages { get; }
    public int? CurrentPage { get; }
    public int? PreviousPage { get; }
    public int? NextPage { get; }

    public LargeSetPagination(int totalPages, int currentPage)
    {
        Show = totalPages > 1;
        if (!Show)
        {
            PaginationItems = Enumerable.Empty<PaginationItem>();
            return;
        }

        PaginationItems = GetPaginationItems(totalPages, currentPage, 1, currentPage - 1, currentPage, currentPage + 1, totalPages);
        TotalPages = totalPages;
        CurrentPage = currentPage;
        PreviousPage = currentPage > 1 ? currentPage-1 : null;
        NextPage = currentPage < totalPages ? currentPage + 1 : null;
    }

    public static IEnumerable<PaginationItem> GetPaginationItems(int totalPages, int currentPage, params int[] pages)
    {
        var uniquePageNumbers = pages.Distinct().Where(p => p > 0 && p <= totalPages);

        var lastPageNumber = 1;
        foreach (var uniquePage in uniquePageNumbers)
        {
            if (uniquePage > lastPageNumber + 1)
            {
                yield return new PaginationItem();
            }

            lastPageNumber = uniquePage;
            yield return new PaginationItem(uniquePage, uniquePage == currentPage);
        }
    }
}

public class DontShowPagination : IPagination
{
    public IEnumerable<PaginationItem> PaginationItems => Enumerable.Empty<PaginationItem>();
    public bool Show => false;
    public int? TotalPages => 1;
    public int? CurrentPage => 1;
    public int? PreviousPage => 1;
    public int? NextPage => 1;
}