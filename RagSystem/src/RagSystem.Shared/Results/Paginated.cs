namespace RagSystem.Shared.Results
{
    public record Paginated<T>
    {
        public IEnumerable<T> Items { get; private set; } = [];
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; set; }
        public long TotalRecords { get; set; }

        public Paginated(IEnumerable<T> items, long totalRecords, int pageNumber, int pageSize)
        {
            Items = items;
            TotalRecords = totalRecords;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        }
    }
}
