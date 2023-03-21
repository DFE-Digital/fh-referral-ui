namespace FamilyHubs.ReferralUi.Ui.Models;

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

        int lastPageNumber = 1;
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