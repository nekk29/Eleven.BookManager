using Eleven.BookManager.Business.Models.Base;

namespace Eleven.BookManager.Business.Models
{
    public class SearchLibraryModel(
        IEnumerable<AuthorModel> items,
        int total,
        int page,
        int pageSize
    ) : SearchResultModel<AuthorModel>(items, total, page, pageSize)
    {
        public int TotalBooks { get; set; }
    }
}
