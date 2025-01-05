namespace Eleven.BookManager.Business.Models
{
    public class SearchLibraryFilter
    {
        public string? Query { get; set; }
        public bool OnlyUnsent { get; set; }
        public bool ShowRecent { get; set; }
    }
}
