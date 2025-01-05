using Eleven.BookManager.Business.Models.Base;

namespace Eleven.BookManager.Business.Contracts
{
    public interface IAmazonService
    {
        Task<ResultModel> SendAuthorkUsingMail(Guid authorId, Action<int, int, string> onProgress);
        Task<ResultModel> SendBookUsingMail(Guid authorBookId, string emailTo = null!);
        Task<ResultModel> MarkAuthorAsSent(Guid authorId, bool sent);
        Task<ResultModel> MarkBookAsSent(Guid authorBookId, bool sent);
        Task<ResultModel> MarkBookAsPending(Guid authorBookId, bool pending);
        Task<ResultModel> DeleteBook(Guid authorBookId);
    }
}
