using Eleven.BookManager.Entity.Base;

namespace Eleven.BookManager.Entity
{
    public class Book : SystemEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string NormalizedTitle { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public bool Sent { get; set; } = false;
        public bool Pending { get; set; } = false;
        public virtual ICollection<AuthorBook> AuthorBooks { get; set; } = [];
    }
}
