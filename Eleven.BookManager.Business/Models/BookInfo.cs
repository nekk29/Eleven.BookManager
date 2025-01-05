using Eleven.BookManager.Business.Extensions;

namespace Eleven.BookManager.Business.Models
{
    public class BookInfo
    {
        public string Title { get; set; } = null!;
        public string NormalizedTitle => Normalize(Title);
        public string FilePath { get; set; } = null!;
        public IEnumerable<string> AuthorList { get; set; } = [];
        public IEnumerable<string> NormalizedAuthorList => AuthorList.Select(Normalize);
        public static string Normalize(string value) => value?.Clear().Trim().ToUpper()!;
    }
}
