using Eleven.BookManager.Business.Models;
using Eleven.BookManager.Business.Models.Base;

namespace Eleven.BookManager.Business.Contracts
{
    public interface ICalibreService
    {
        Task<BookInfo> GetBookInfo(Guid authorBookId);
        Task<ResultModel> Sync(Action<int, int> onProgress);
        Task<ResultModel<SearchLibraryModel>> SearchLibrary(SearchParamsModel<SearchLibraryFilter> searchParams);
    }
}
