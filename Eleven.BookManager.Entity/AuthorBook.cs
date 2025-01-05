using Eleven.BookManager.Entity.Base;

namespace Eleven.BookManager.Entity
{
    public class AuthorBook : SystemEntity
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public Author Author { get; set; } = null!;
        public Guid BookId { get; set; }
        public Book Book { get; set; } = null!;
    }
}
