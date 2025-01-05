namespace Eleven.BookManager.Business.Models.Base
{
    public class SearchResultModel<TResult>
    {
        public SearchResultModel(IEnumerable<TResult> items)
        {
            Items = items;
            Total = Items.Count();
            Page = 1;
            PageSize = Items.Count();
        }

        public SearchResultModel(IEnumerable<TResult> items, int total) : this(items)
        {
            Total = total > Items.Count() ? total : Items.Count();
            Page = 1;
            PageSize = Items.Count();
        }

        public SearchResultModel(IEnumerable<TResult> items, int total, int page, int pageSize) : this(items, total)
        {
            Page = page;
            PageSize = pageSize;
        }

        public SearchResultModel(IEnumerable<TResult> items, int total, SearchParamsModel? searchParams) : this(items, total)
        {
            Page = searchParams?.Page?.Page ?? 1;
            PageSize = searchParams?.Page?.PageSize ?? Items.Count();
        }

        public IEnumerable<TResult> Items { get; set; } = [];
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
