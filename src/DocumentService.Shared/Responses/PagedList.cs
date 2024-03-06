namespace DocumentService.Shared.Responses;

public sealed class PagedList<T>
{
    private PagedList(int totalItems, IReadOnlyCollection<T> items)
    { 
        TotalItems = totalItems;
        Items = items;
    }

    public int TotalItems { get; init; }
    public IReadOnlyCollection<T> Items { get; init; }

    public static PagedList<T> Create(int totalItems, IReadOnlyCollection<T> items) =>
       new PagedList<T>(totalItems, items);
}
