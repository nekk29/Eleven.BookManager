namespace Eleven.BookManager.Business.Models
{
    public class BookModel
    {
        public Guid AuthorBookId { get; set; }
        public Guid BookId { get; set; }
        public string Title { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public bool Sent { get; set; }
        public bool Pending { get; set; }
    }
}
