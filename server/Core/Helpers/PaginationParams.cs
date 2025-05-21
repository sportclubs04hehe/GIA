namespace Core.Helpers
{
    public class PaginationParams
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? OrderBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }
}
