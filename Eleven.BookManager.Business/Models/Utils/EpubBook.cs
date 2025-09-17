namespace Eleven.BookManager.Business.Models.Utils
{
    public class EpubBook
    {
        public string Title { get; set; } = null!;
        public IEnumerable<string> Authors { get; set; } = [];
        public string Description { get; set; } = null!;
        public byte[] CoverImage { get; set; } = [];
    }
}
