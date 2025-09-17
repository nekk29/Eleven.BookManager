using Eleven.BookManager.Business.Contracts;
using Eleven.BookManager.Business.Contracts.Configuration;
using Eleven.BookManager.Business.Contracts.Utils;
using Eleven.BookManager.Business.Models;
using Eleven.BookManager.Business.Models.Base;
using Eleven.BookManager.Entity;
using Eleven.BookManager.Entity.Base;
using Eleven.BookManager.Repository.Contracts;
using Eleven.BookManager.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Eleven.BookManager.Business
{
    public class CalibreService(
        IEpubReader epubReader,
        IRepository<Book> bookRepository,
        IRepository<Author> authorRepository,
        IRepository<AuthorBook> authorBookRepository,
        IBusinessConfiguration configuration
    ) : ICalibreService
    {
        private readonly IEpubReader _epubReader = epubReader;
        private readonly IRepository<Book> _bookRepository = bookRepository;
        private readonly IRepository<Author> _authorRepository = authorRepository;
        private readonly IRepository<AuthorBook> _authorBookRepository = authorBookRepository;
        private readonly IBusinessConfiguration _configuration = configuration;

        public async Task<ResultModel> Sync(Action<int, int> onProgress)
        {
            var result = new ResultModel();
            var calibreOptions = _configuration.GetCalibreOptions();

            if (!Directory.Exists(calibreOptions.LibraryDirectory))
            {
                result.AddErrorResult(string.Format(Resources.CalibreMessages.SyncDirectoryNotFound, calibreOptions.LibraryDirectory));
                return result;
            }

            var filePaths = Directory
                .GetFiles(calibreOptions.LibraryDirectory, "*.epub", SearchOption.AllDirectories)
                .Where(x => !x.Contains("trash"));

            var counter = 0;
            var filesCount = filePaths.Count();
            var bookInfos = new List<BookInfo>();

            foreach (var filePath in filePaths)
            {
                try
                {
                    var bookInfo = await GetBookInfoFromFile(result, filePath);
                    if (bookInfo == null) continue;

                    bookInfos.Add(bookInfo);

                    foreach (var authorName in bookInfo.AuthorList)
                    {
                        var author = await CreateOrUpdateAuthor(authorName);
                        var book = await CreateOrUpdateBook(bookInfo);
                        await CreateAuthorBook(author, book);
                    }
                }
                catch
                {
                    result.AddErrorResult(string.Format(Resources.CalibreMessages.SyncFileError, filePath));
                }

                counter++;

                onProgress?.Invoke(filesCount, counter);
            }

            await CleanDeletedData(bookInfos);

            if (result.IsValid)
                result.AddOkResult(Resources.CalibreMessages.SyncSuccess);

            return result;
        }

        private async Task CleanDeletedData(List<BookInfo> bookInfos)
        {
            var bookFilePaths = bookInfos.Select(x => x.FilePath);
            var deletedBooks = await _bookRepository.FindByAsync(x => !bookFilePaths.Contains(x.FilePath));

            foreach (var deletedBook in deletedBooks)
            {
                var authorBooks = await _authorBookRepository.FindByAsync(x => x.BookId == deletedBook.Id);

                foreach (var authorBook in authorBooks ?? [])
                {
                    await _authorBookRepository.DeleteAsync(authorBook);
                    await _authorBookRepository.SaveAsync();
                }

                await _bookRepository.DeleteAsync(deletedBook);
                await _bookRepository.SaveAsync();
            }

            var authorNames = new List<string>();
            bookInfos.ForEach(x => authorNames.AddRange(x.NormalizedAuthorList));
            authorNames = authorNames.Distinct().ToList();

            var deletedAuthors = await _authorRepository.FindByAsync(x => !authorNames.Contains(x.NormalizedName));

            foreach (var deletedAuthor in deletedAuthors)
            {
                var authorBooks = await _authorBookRepository.FindByAsync(x => x.AuthorId == deletedAuthor.Id);

                foreach (var authorBook in authorBooks ?? [])
                {
                    await _authorBookRepository.DeleteAsync(authorBook);
                    await _authorBookRepository.SaveAsync();
                }

                await _authorRepository.DeleteAsync(deletedAuthor);
                await _authorRepository.SaveAsync();
            }
        }

        public async Task<BookInfo> GetBookInfo(Guid authorBookId)
        {
            var calibreOptions = _configuration.GetCalibreOptions();

            var authorBook = await _authorBookRepository.GetByAsync(b => b.Id == authorBookId);
            if (authorBook == null) return null!;

            var book = await _bookRepository.GetByAsync(b => b.Id == authorBook.BookId);
            if (book == null) return null!;

            var filePath = Path.Combine(calibreOptions.LibraryDirectory, book.FilePath);
            var bookInfo = book == null ? null! : await _epubReader.Read(filePath);

            return bookInfo ?? await _epubReader.ReadFromPath(filePath);
        }

        private async Task<BookInfo> GetBookInfoFromFile(ResultModel result, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                result.AddErrorResult(Resources.AmazonMessages.BookFileEmpty);
                return null!;
            }

            if (!File.Exists(filePath))
            {
                result.AddErrorResult(string.Format(Resources.AmazonMessages.BookFileNotFound, filePath));
                return null!;
            }

            try
            {
                var bookInfo = await _epubReader.Read(filePath) ?? throw new Exception("Epub was not properly read");
                return bookInfo;
            }
            catch
            {
                var bookInfo = await _epubReader.ReadFromPath(filePath);
                if (bookInfo == null)
                {
                    result.AddErrorResult(string.Format(Resources.CalibreMessages.SyncBookError, filePath));
                    return null!;
                }

                return bookInfo;
            }
        }

        private async Task<Author> CreateOrUpdateAuthor(string authorName)
        {
            var normalizedAuthorName = BookInfo.Normalize(authorName);
            var author = await _authorRepository.GetByAsync(b => b.NormalizedName == normalizedAuthorName);

            if (author == null)
            {
                author = new Author
                {
                    Name = authorName,
                    NormalizedName = normalizedAuthorName
                };

                await _authorRepository.AddAsync(author);
            }
            else
            {
                author.Name = authorName;
                author.NormalizedName = normalizedAuthorName;

                await _authorRepository.UpdateAsync(author);
            }

            await _authorRepository.SaveAsync();

            return author;
        }

        private async Task<Book> CreateOrUpdateBook(BookInfo bookInfo)
        {
            var book = await _bookRepository.GetByAsync(b =>
                b.NormalizedTitle == bookInfo.NormalizedTitle &&
                b.FilePath == bookInfo.FilePath
            );

            if (book == null)
            {
                book = new Book
                {
                    Title = bookInfo.Title,
                    NormalizedTitle = bookInfo.NormalizedTitle,
                    FilePath = bookInfo.FilePath,
                    Sent = false,
                };

                await _bookRepository.AddAsync(book);
            }
            else
            {
                if (book.Sent) return book;

                book.Title = bookInfo.Title;
                book.NormalizedTitle = bookInfo.NormalizedTitle;
                book.FilePath = bookInfo.FilePath;

                await _bookRepository.UpdateAsync(book);
            }

            await _bookRepository.SaveAsync();

            return book;
        }

        private async Task CreateAuthorBook(Author author, Book book)
        {
            if (author == null || book == null) return;

            var authorBook = await _authorBookRepository.GetByAsync(x => x.AuthorId == author.Id && x.BookId == book.Id);
            if (authorBook != null) return;

            authorBook = new AuthorBook
            {
                AuthorId = author.Id,
                Author = null!,
                BookId = book.Id,
                Book = null!,
            };

            await _authorBookRepository.AddAsync(authorBook);
            await _authorBookRepository.SaveAsync();
        }

        public async Task<ResultModel<SearchLibraryModel>> SearchLibrary(SearchParamsModel<SearchLibraryFilter> searchParams)
        {
            var result = new ResultModel<SearchLibraryModel>();

            searchParams ??= new SearchParamsModel<SearchLibraryFilter>();
            searchParams.Filter ??= new SearchLibraryFilter();
            searchParams.Page ??= new PageParamsModel();

            var page = searchParams.Page;
            var sortParams = searchParams.SortParams ?? [];
            var query = BookInfo.Normalize(searchParams.Filter.Query!);
            var onlyUnsent = searchParams.Filter.OnlyUnsent;
            var showRecent = searchParams.Filter.ShowRecent;

            #region Authors
            Expression<Func<Author, bool>> authorFilter = x => true;

            if (string.IsNullOrEmpty(query))
            {
                if (onlyUnsent) authorFilter = authorFilter.And(x => x.AuthorBooks.Any(b => !b.Book.Sent));
            }
            else
            {
                if (onlyUnsent)
                {
                    authorFilter = authorFilter.And(x =>
                        (x.NormalizedName.Contains(query) && x.AuthorBooks.Any(b => !b.Book.Sent)) ||
                        (!x.NormalizedName.Contains(query) && x.AuthorBooks.Any(b => !b.Book.Sent && b.Book.NormalizedTitle.Contains(query)))
                    );
                }
                else
                {
                    authorFilter = authorFilter.And(x =>
                        x.NormalizedName.Contains(query) || x.AuthorBooks.Any(b => b.Book.NormalizedTitle.Contains(query))
                    );
                }
            }

            var sorts = new List<SortExpression<Author>>();

            if (showRecent)
                sorts.Add(new SortExpression<Author> { Property = x => x.CreationDate, Direction = SortDirection.Desc });
            else
                sorts.Add(new SortExpression<Author> { Property = x => x.NormalizedName, Direction = SortDirection.Asc });

            var authors = await _authorRepository.SearchByAsNoTrackingAsync(
                page.Page,
                page.PageSize,
                sorts,
                authorFilter
            );
            #endregion

            #region Books
            var authorIds = await _authorRepository
                .FindAll()
                .Where(authorFilter)
                .Select(x => x.Id)
                .ToListAsync();

            var totalBooks = await _authorBookRepository
                .FindAll()
                .Where(GetAuthorBookFilter(authorIds, query, onlyUnsent))
                .Select(x => x.BookId)
                .Distinct()
                .CountAsync();

            authorIds = authors.Items?.Select(x => x.Id).ToList() ?? [];

            var allAuthorBooks = await _authorBookRepository
                .FindAll()
                .Where(GetAuthorBookFilter(authorIds, query, onlyUnsent))
                .Select(x => new
                {
                    x.Id,
                    x.AuthorId,
                    x.BookId,
                    x.Book.Title,
                    x.Book.NormalizedTitle,
                    x.Book.FilePath,
                    x.Book.Sent,
                    x.Book.Pending,
                    x.Book.CreationDate,
                })
                .ToListAsync();

            var authorModels = new List<AuthorModel>();

            foreach (var author in authors.Items ?? [])
            {
                var authorBooks = allAuthorBooks.Where(x => x.AuthorId == author.Id);

                if (showRecent)
                    authorBooks = authorBooks.OrderByDescending(x => x.CreationDate);
                else
                    authorBooks = authorBooks.OrderBy(x => x.NormalizedTitle);

                authorModels.Add(new AuthorModel
                {
                    Id = author.Id,
                    Name = author.Name,
                    Books = authorBooks.Select(x => new BookModel
                    {
                        AuthorBookId = x.Id,
                        BookId = x.BookId,
                        Title = x.Title,
                        FilePath = x.FilePath,
                        Sent = x.Sent,
                        Pending = x.Pending,
                    }).ToList(),
                });
            }
            #endregion

            var searchLibraryModel = new SearchLibraryModel(
                authorModels,
                authors.Total,
                page.Page,
                page.PageSize
            )
            { TotalBooks = totalBooks };

            result.UpdateData(searchLibraryModel);

            return result;
        }

        private static Expression<Func<AuthorBook, bool>> GetAuthorBookFilter(IEnumerable<Guid> authorIds, string _query, bool _onlyUnsent)
        {
            Expression<Func<AuthorBook, bool>> authorBookFilter = x => authorIds.Contains(x.AuthorId);

            if (string.IsNullOrEmpty(_query))
            {
                if (_onlyUnsent) authorBookFilter = authorBookFilter.And(x => !x.Book.Sent);
            }
            else
            {
                if (_onlyUnsent)
                    authorBookFilter = authorBookFilter.And(x => (x.Author.NormalizedName.Contains(_query) || x.Book.NormalizedTitle.Contains(_query)) && !x.Book.Sent);
                else
                    authorBookFilter = authorBookFilter.And(x => x.Author.NormalizedName.Contains(_query) || x.Book.NormalizedTitle.Contains(_query));
            }

            return authorBookFilter;
        }
    }
}
