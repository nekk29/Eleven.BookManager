using Eleven.BookManager.Business.Contracts.Configuration;
using Eleven.BookManager.Business.Contracts.Utils;
using Eleven.BookManager.Business.Extensions;
using Eleven.BookManager.Business.Models;
using Eleven.BookManager.Business.Models.Configuration;
using Eleven.BookManager.Business.Models.Utils;

namespace Eleven.BookManager.Business.Utils
{
    public class EpubReader(IBusinessConfiguration configuration) : IEpubReader
    {
        private readonly CalibreOptions _calibreOptions = configuration.GetCalibreOptions();

        public async Task<BookInfo> Read(string filePath)
        {
            var epubBook = await GetUsingVersOne(filePath);
            if (epubBook != null) return epubBook;

            epubBook = await GetUsingEpubSharp(filePath);
            if (epubBook != null) return epubBook;

            epubBook = await GetUsingEpubNet(filePath);
            if (epubBook != null) return epubBook;

            epubBook = await GetUsingEpubCore(filePath);
            if (epubBook != null) return epubBook;

            return null!;
        }

        public async Task<BookInfo> ReadFromPath(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var fileName = fileInfo?.Name ?? string.Empty;
            var dashIndex = fileName.IndexOf('-');

            var title = fileName?[..dashIndex]?.Trim();
            var author = fileName?.Substring(dashIndex + 1, fileName.Length - dashIndex - 1).Replace(".epub", "").Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(author))
                return null!;

            return await Task.FromResult(new BookInfo
            {
                Title = title,
                FilePath = _calibreOptions.GetRelativePath(filePath),
                AuthorList = [author]
            });
        }

        private async Task<BookInfo> GetUsingVersOne(string filePath)
        {
            return await BookFromMetadata(filePath, async (path) =>
            {
                using var stream = GetStreamFromFile(filePath);
                var epubBook = await VersOne.Epub.EpubReader.OpenBookAsync(stream, VersOne.Epub.Options.EpubReaderOptionsPreset.IGNORE_ALL_ERRORS);

                return new EpubBook
                {
                    Title = epubBook.Title,
                    Authors = epubBook.AuthorList,
                    Description = epubBook.Description ?? string.Empty,
                    CoverImage = epubBook.ReadCover() ?? []
                };
            });
        }

        private async Task<BookInfo> GetUsingEpubSharp(string filePath)
        {
            return await BookFromMetadata(filePath, async (path) =>
            {
                using var stream = GetStreamFromFile(filePath);
                var epubBook = EpubSharp.EpubReader.Read(stream, false);

                return await Task.FromResult(new EpubBook
                {
                    Title = epubBook.Title,
                    Authors = epubBook.Authors,
                    Description = string.Empty,
                    CoverImage = epubBook.CoverImage
                });
            });
        }

        private async Task<BookInfo> GetUsingEpubNet(string filePath)
        {
            return await BookFromMetadata(filePath, async (path) =>
            {
                using var stream = GetStreamFromFile(filePath);
                var epubBook = await EpubNet.EpubReader.ReadBookAsync(stream);

                return new EpubBook
                {
                    Title = epubBook.Title,
                    Authors = epubBook.AuthorList,
                    Description = string.Empty,
                    CoverImage = epubBook.CoverImage
                };
            });
        }

        private async Task<BookInfo> GetUsingEpubCore(string filePath)
        {
            return await BookFromMetadata(filePath, async (path) =>
            {
                using var stream = GetStreamFromFile(filePath);
                var epubBook = EpubCore.EpubReader.Read(stream, false);

                return await Task.FromResult(new EpubBook
                {
                    Title = epubBook.Title,
                    Authors = epubBook.Authors,
                    Description = string.Empty,
                    CoverImage = epubBook.CoverImage
                });
            });
        }

        private async Task<BookInfo> BookFromMetadata(string filePath, Func<string, Task<EpubBook>> readEpubAction)
        {
            try
            {
                var epubResul = await readEpubAction.Invoke(filePath);

                return new BookInfo
                {
                    Title = epubResul.Title,
                    FilePath = _calibreOptions.GetRelativePath(filePath),
                    AuthorList = epubResul.Authors ?? [],
                    Description = epubResul.Description,
                    CoverImage = epubResul.CoverImage
                };
            }
            catch
            {
                return null!;
            }
        }

        private static FileStream GetStreamFromFile(string filePath)
            => new(filePath, FileMode.Open, FileAccess.Read);
    }
}
