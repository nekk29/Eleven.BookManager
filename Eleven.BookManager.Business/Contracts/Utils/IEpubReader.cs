using Eleven.BookManager.Business.Models;

namespace Eleven.BookManager.Business.Contracts.Utils
{
    public interface IEpubReader
    {
        Task<BookInfo> Read(string filePath);
        Task<BookInfo> ReadFromPath(string filePath);
    }
}
