namespace Eleven.BookManager.Business.Models.Utils
{
    public class EpubBook
    {
        public string Title { get; set; } = null!;
        public IEnumerable<string> Authors { get; set; } = [];
    }
}
