namespace Eleven.BookManager.Business.Models.Base
{
    public class SearchParamsModel
    {
        public PageParamsModel? Page { get; set; }
        public IEnumerable<SortParamsModel>? SortParams { get; set; }
    }

    public class SearchParamsModel<TFilter> : SearchParamsModel
    {
        public TFilter? Filter { get; set; }
    }
}
