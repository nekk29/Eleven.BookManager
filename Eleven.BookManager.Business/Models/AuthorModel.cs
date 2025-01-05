namespace Eleven.BookManager.Business.Models
{
    public class AuthorModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<BookModel> Books { get; set; } = [];
    }
}
