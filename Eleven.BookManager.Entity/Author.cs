using Eleven.BookManager.Entity.Base;

namespace Eleven.BookManager.Entity
{
    public class Author : SystemEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string NormalizedName { get; set; } = null!;
        public virtual ICollection<AuthorBook> AuthorBooks { get; set; } = [];
    }
}
