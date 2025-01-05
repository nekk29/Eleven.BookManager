using Eleven.BookManager.Business.Contracts;
using Eleven.BookManager.Business.Contracts.Configuration;
using Eleven.BookManager.Business.Contracts.Utils;
using Eleven.BookManager.Business.Extensions;
using Eleven.BookManager.Business.Models.Base;
using Eleven.BookManager.Entity;
using Eleven.BookManager.Repository.Contracts;

namespace Eleven.BookManager.Business
{
    public class AmazonService(
        IEmailClient emailClient,
        IRepository<Book> bookRepository,
        IRepository<Author> authorRepository,
        IRepository<AuthorBook> authorBookRepository,
        IBusinessConfiguration configuration
    ) : IAmazonService
    {
        private readonly IEmailClient _emailClient = emailClient;
        private readonly IRepository<Book> _bookRepository = bookRepository;
        private readonly IRepository<Author> _authorRepository = authorRepository;
        private readonly IRepository<AuthorBook> _authorBookRepository = authorBookRepository;
        private readonly IBusinessConfiguration _configuration = configuration;

        public async Task<ResultModel> SendAuthorkUsingMail(Guid authorId, Action<int, int, string> onProgress)
        {
            var result = new ResultModel();
            var amazonOptions = _configuration.GetAmazonOptions();
            var calibreOptions = _configuration.GetCalibreOptions();

            var authorBooks = await _authorBookRepository.FindByAsync(x => x.AuthorId == authorId && !x.Book.Sent, x => x.Book);
            if (authorBooks == null || !authorBooks.Any()) return null!;

            var emailTo = amazonOptions?.AccountEmail!;
            if (string.IsNullOrEmpty(emailTo))
            {
                result.AddErrorResult(Resources.AmazonMessages.AccountEmailNotFound);
                return result;
            }

            var counter = 0;
            var booksCount = authorBooks.Count();

            foreach (var authorBook in authorBooks.OrderBy(X => X.Book.Title))
            {
                counter++;
                onProgress?.Invoke(booksCount, counter, authorBook.Book?.Title!);

                var partialResult = await SendBookUsingMail(authorBook.Id, emailTo);

                if (!partialResult.IsValid)
                    result.AttachResults(partialResult);
            }

            if (result.IsValid)
                result.AddOkResult(Resources.AmazonMessages.SendAuthorSuccess);

            return result;
        }

        public async Task<ResultModel> SendBookUsingMail(Guid authorBookId, string emailTo = null!)
        {
            var result = new ResultModel();
            var amazonOptions = _configuration.GetAmazonOptions();
            var calibreOptions = _configuration.GetCalibreOptions();

            if (string.IsNullOrEmpty(emailTo))
            {
                emailTo = amazonOptions?.AccountEmail!;

                if (string.IsNullOrEmpty(emailTo))
                {
                    result.AddErrorResult(Resources.AmazonMessages.AccountEmailNotFound);
                    return result;
                }
            }

            var authorBook = await _authorBookRepository.GetByAsync(x => x.Id == authorBookId, x => x.Author);
            if (authorBook?.Author == null)
            {
                result.AddErrorResult(Resources.AmazonMessages.AuthorNotFound);
                return result;
            }

            var book = await _bookRepository.GetByAsync(x => x.Id == authorBook.BookId);
            if (book == null)
            {
                result.AddErrorResult(Resources.AmazonMessages.BookNotFound);
                return result;
            }

            if (book.Sent)
            {
                result.AddOkResult(Resources.AmazonMessages.SendBookAlreadySent);
                return result;
            }

            if (book.Pending)
            {
                result.AddWarningResult(string.Format(Resources.AmazonMessages.SendBookWarningPending, $"{authorBook.Author.Name} - {book.Title}"));
                return result;
            }

            var filePath = Path.Combine(calibreOptions.LibraryDirectory, book.FilePath);
            if (!File.Exists(filePath))
            {
                result.AddErrorResult(string.Format(Resources.AmazonMessages.BookFileNotFound, filePath));
                return result;
            }

            var bookFileName = book.Title.GetValidFileName();
            var tempDirectory = Directory.CreateTempSubdirectory();
            var tempFileName = Path.Combine(tempDirectory.FullName, $"{bookFileName}.epub");

            File.Copy(filePath, tempFileName);

            try
            {
                await _emailClient.SendEmailAsync(
                    emailTo!,
                    tempFileName,
                    "convert",
                    $"Book: {authorBook.Author.Name} - {book.Title}"
                );
            }
            catch (Exception ex)
            {
                result.AddErrorResult(string.Format(Resources.AmazonMessages.SendEmailError, $"{authorBook.Author.Name} - {book.Title}", ex.Message));
                return result;
            }

            if (File.Exists(tempFileName))
                File.Delete(tempFileName);

            book.Sent = true;

            await _bookRepository.UpdateAsync(book);
            await _bookRepository.SaveAsync();

            result.AddOkResult(Resources.AmazonMessages.SendBookSuccess);
            return result;
        }

        public async Task<ResultModel> MarkAuthorAsSent(Guid authorId, bool sent)
        {
            var result = new ResultModel();

            try
            {
                await UpdateAuthorBooks(authorId, result, (book) =>
                {
                    book.Sent = sent;
                    if (sent) book.Pending = false;
                });


                if (result.IsValid)
                    result.AddOkResult(Resources.AmazonMessages.MarkAuthorSuccess);
            }
            catch (Exception ex)
            {
                result.AddErrorResult(string.Format(Resources.AmazonMessages.MarkAuthorError, ex.Message));
                return result;
            }

            return result;
        }

        public async Task<ResultModel> MarkBookAsSent(Guid authorBookId, bool sent)
        {
            var result = new ResultModel();

            try
            {
                await UpdateIndividualBook(authorBookId, result, (book) =>
                {
                    book.Sent = sent;
                    if (sent) book.Pending = false;
                });

                if (result.IsValid)
                    result.AddOkResult(Resources.AmazonMessages.MarkBookSuccess);
            }
            catch (Exception ex)
            {
                result.AddErrorResult(string.Format(Resources.AmazonMessages.MarkBookError, ex.Message));
                return result;
            }

            return result;
        }

        public async Task<ResultModel> MarkBookAsPending(Guid authorBookId, bool pending)
        {
            var result = new ResultModel();

            try
            {
                await UpdateIndividualBook(authorBookId, result, (book) =>
                {
                    book.Pending = pending;
                    if (pending) book.Sent = false;
                });

                if (result.IsValid)
                    result.AddOkResult(Resources.AmazonMessages.MarkBookSuccess);
            }
            catch (Exception ex)
            {
                result.AddErrorResult(string.Format(Resources.AmazonMessages.MarkBookError, ex.Message));
                return result;
            }

            return result;
        }

        public async Task<ResultModel> DeleteBook(Guid authorBookId)
        {
            var result = new ResultModel();

            try
            {
                var authorBook = await _authorBookRepository.GetByAsync(x => x.Id == authorBookId);
                if (authorBook == null)
                {
                    result.AddErrorResult(Resources.AmazonMessages.BookNotFound);
                    return result;
                }

                var book = await _bookRepository.GetByAsync(x => x.Id == authorBook.BookId);

                if (book == null)
                {
                    result.AddErrorResult(Resources.AmazonMessages.BookNotFound);
                    return result;
                }

                if (book.Sent)
                {
                    result.AddErrorResult(Resources.AmazonMessages.DeleteBookErrorAlreadySent);
                    return result;
                }

                await _authorBookRepository.DeleteAsync(authorBook);
                await _authorBookRepository.SaveAsync();

                await _bookRepository.DeleteAsync(book);
                await _bookRepository.SaveAsync();

                result.AddOkResult(Resources.AmazonMessages.DeleteBookSuccess);
            }
            catch (Exception ex)
            {
                result.AddErrorResult(string.Format(Resources.AmazonMessages.DeleteBookError, ex.Message));
                return result;
            }

            return result;
        }

        public async Task UpdateAuthorBooks(Guid authorId, ResultModel result, Action<Book> update)
        {
            var author = await _authorRepository.GetByAsync(x => x.Id == authorId);
            if (author == null)
            {
                result.AddErrorResult(Resources.AmazonMessages.AuthorNotFound);
                return;
            }

            var books = await _bookRepository.FindByAsync(x => x.AuthorBooks.Any(x => x.AuthorId == author.Id));
            if (books == null)
            {
                result.AddErrorResult(Resources.AmazonMessages.BookNotFound);
                return;
            }

            foreach (var book in books)
            {
                update?.Invoke(book);
                await _bookRepository.UpdateAsync(book);
                await _bookRepository.SaveAsync();
            }
        }

        public async Task UpdateIndividualBook(Guid authorBookId, ResultModel result, Action<Book> update)
        {
            var authorBook = await _authorBookRepository.GetByAsync(x => x.Id == authorBookId);
            if (authorBook == null)
            {
                result.AddErrorResult(Resources.AmazonMessages.BookNotFound);
                return;
            }

            var book = await _bookRepository.GetByAsync(x => x.Id == authorBook.BookId);
            if (book == null)
            {
                result.AddErrorResult(Resources.AmazonMessages.BookNotFound);
                return;
            }

            update?.Invoke(book);
            await _bookRepository.UpdateAsync(book);
            await _bookRepository.SaveAsync();
        }
    }
}
