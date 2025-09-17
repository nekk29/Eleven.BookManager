using Eleven.BookManager.App.ViewModels.Base;
using Eleven.BookManager.Business.Models;
using Eleven.BookManager.Business.Models.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using UraniumUI.Material.Controls;

namespace Eleven.BookManager.App.ViewModels.MainPage
{
    public partial class MainPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public Command ToogleNodeCommand => new(ToogleNodeCommandHandler);
        public void ToogleNodeCommandHandler(object node)
        {
            if (node is TreeViewNodeHolderView nodeView) nodeView.IsExpanded = !nodeView.IsExpanded;
        }

        public Command ChangePageCommand => new(ChangePageHandler);
        public async void ChangePageHandler(object currentPage)
        {
            CurrentPage = currentPage as int? ?? 1;

            await SearchLibrary(Query, OnlyUnsent, ShowRecent, () =>
            {
                var mainPage = ContentPage as Pages.MainPage;
                mainPage?.ResetTreeStatus();

            });
        }

        public Command SyncLibraryCommand => new(async () => await SyncLibraryHandler());
        public async Task SyncLibraryHandler()
        {
            try
            {
                SyncProgress = 0;
                SyncProgressPerc = 0;

                IsBusy = true;
                IsSyncing = true;

                MarkLibraryAsBusy(true);

                var result = await CalibreService.Sync(
                    (max, progress) =>
                    {
                        SyncProgress = (double)progress / max;
                        SyncProgressPerc = SyncProgress * 100;
                    }
                );

                IsSyncing = false;
                IsBusy = false;

                await RefreshLibrary();
                await ContentPage?.DisplayAlert("Result", result.GetFormattedMessages(true), "Ok")!;
            }
            catch (Exception ex)
            {
                await ContentPage?.DisplayAlert("Error", ex.Message, "Ok")!;
            }
            finally
            {
                MarkLibraryAsBusy(false);
                IsSyncing = false;
                IsBusy = false;
            }
        }

        public Command ViewBookCommand => new(ViewBookCommandHandler);
        public async void ViewBookCommandHandler(object libraryModelObj)
        {
            try
            {
                if (libraryModelObj is not LibraryViewModel libraryViewModel) return;
                if (!libraryViewModel.IsBook) return;

                var bookInfo = await CalibreService.GetBookInfo(libraryViewModel.AuthorBookId!.Value);

                BookDescriptionText = string.IsNullOrEmpty(bookInfo.Description)
                    ? bookInfo.Title : bookInfo.Description;

                var bookCoverStream = bookInfo.CoverImage;

                if (bookCoverStream != null && bookCoverStream.Length > 0)
                {
                    var memoryStream = new MemoryStream(bookCoverStream);
                    BookImageSource = ImageSource.FromStream(() => memoryStream);

                }
                else
                    BookImageSource = ImageSource.FromFile("cover.jpg");

            }
            catch
            {
                BookImageSource = ImageSource.FromFile("cover.jpg");
                BookDescriptionText = _bookDescription;
            }
        }

        public Command SendUsingEmailCommand => new(SendUsingEmailHandler);
        public async void SendUsingEmailHandler(object libraryModelObj)
        {
            if (libraryModelObj is not LibraryViewModel libraryViewModel) return;

            await ExecuteLibraryModelAction(
                libraryViewModel,
                async (model) =>
                {
                    model.SendProgress = 0;
                    model.SendProgressPerc = 0;
                    model.SendProgressText = string.Empty;
                    model.IsSendingAuthor = true;

                    return await AmazonService.SendAuthorkUsingMail(
                        model.AuthorId!.Value,
                        (max, progress, text) =>
                        {
                            model.SendProgress = (double)progress / max;
                            model.SendProgressPerc = model.SendProgress * 100;
                            model.SendProgressText = $"{model.SendProgressPerc:#.00}% - Sending: {text}";
                        }
                    );
                },
                () =>
                {
                    libraryViewModel.IsSendingAuthor = false;
                },
                async (model) =>
                {
                    model.IsSendingBook = true;
                    return await AmazonService.SendBookUsingMail(model.AuthorBookId!.Value);
                },
                () =>
                {
                    libraryViewModel.IsSendingBook = false;
                }
            );
        }

        public Command MarkAsSentCommand => new(MarkAsSentHandler);
        public async void MarkAsSentHandler(object libraryModelObj)
        {
            if (libraryModelObj is not LibraryViewModel libraryViewModel) return;

            await ExecuteLibraryModelAction(
                libraryViewModel,
                async (model) => await AmazonService.MarkAuthorAsSent(model.AuthorId!.Value, !model.Sent), null!,
                async (model) => await AmazonService.MarkBookAsSent(model.AuthorBookId!.Value, !model.Sent), null!
            );
        }

        public Command MarkAsPendingCommand => new(MarkAsPendingHandler);
        public async void MarkAsPendingHandler(object libraryModelObj)
        {
            if (libraryModelObj is not LibraryViewModel libraryViewModel) return;

            await ExecuteLibraryModelAction(
                libraryViewModel,
                async (model) => await Task.FromResult(new ResultModel()), null!,
                async (model) => await AmazonService.MarkBookAsPending(model.AuthorBookId!.Value, !model.Pending), null!
            );
        }

        public Command DeleteBookCommand => new(DeleteBookCommandHandler);
        public async void DeleteBookCommandHandler(object libraryModelObj)
        {
            if (libraryModelObj is not LibraryViewModel libraryViewModel) return;

            await ExecuteLibraryModelAction(
                libraryViewModel,
                async (model) => await Task.FromResult(new ResultModel()), null!,
                async (model) => await AmazonService.DeleteBook(model.AuthorBookId!.Value), null!
            );
        }

        public async Task ExecuteLibraryModelAction(
            LibraryViewModel libraryViewModel,
            Func<LibraryViewModel, Task<ResultModel>> authorAction, Action onAuthorComplete,
            Func<LibraryViewModel, Task<ResultModel>> bookAction, Action onBookComplete
        )
        {
            try
            {
                IsBusy = true;
                MarkLibraryAsBusy(true);

                var result = default(ResultModel);

                if (libraryViewModel.AuthorId.HasValue && authorAction != null)
                    result = await authorAction.Invoke(libraryViewModel);

                if (libraryViewModel.AuthorBookId.HasValue && bookAction != null)
                    result = await bookAction.Invoke(libraryViewModel);

                if (result != null)
                {
                    libraryViewModel.IsBusy = false;
                    IsBusy = false;
                    await RefreshLibrary();
                    await ContentPage?.DisplayAlert("Result", result.GetFormattedMessages(true), "Ok")!;
                }
            }
            catch (Exception ex)
            {
                await ContentPage?.DisplayAlert("Error", ex.Message, "Ok")!;
            }
            finally
            {
                onAuthorComplete?.Invoke();
                onBookComplete?.Invoke();
                MarkLibraryAsBusy(false);
                IsBusy = false;
            }
        }

        public async Task RefreshLibrary() => await SearchLibrary(Query, OnlyUnsent, ShowRecent);

        public async Task SearchLibrary(string query, bool onlyUnsent, bool showRecent, Action onComplete = null!)
        {
            try
            {
                IsSearching = true;
                BookDescriptionText = _bookDescription;
                BookImageSource = ImageSource.FromFile("cover.jpg");

                var filter = new SearchLibraryFilter
                {
                    Query = query,
                    OnlyUnsent = onlyUnsent,
                    ShowRecent = showRecent
                };

                var searchParams = new SearchParamsModel<SearchLibraryFilter>()
                {
                    Filter = filter,
                    Page = new PageParamsModel { Page = CurrentPage, PageSize = PageSize }
                };

                var groupLetter = string.Empty;
                var libraryCollection = new ObservableCollection<LibraryViewModel>();
                var authorBooksResult = await CalibreService.SearchLibrary(searchParams);
                var librarySearchResult = authorBooksResult.Data ?? new SearchLibraryModel([], 0, CurrentPage, PageSize);

                foreach (var authorBook in librarySearchResult.Items ?? [])
                {
                    var books = authorBook.Books ?? [];

                    var authorLibrary = new LibraryViewModel
                    {
                        AuthorId = authorBook.Id,
                        Text = $"{authorBook.Name} ({books.Count})",
                        Books = [],
                        IsBusy = IsBusy
                    };

                    var authorLetter = authorLibrary.GetFirstLetter();
                    if (groupLetter != authorLetter)
                    {
                        groupLetter = authorLetter;
                        authorLibrary.UpdateLetterIcon();
                        authorLibrary.LetterIconIsVisible = true;
                    }

                    foreach (var book in books)
                    {
                        var bookLibrary = new LibraryViewModel
                        {
                            AuthorBookId = book.AuthorBookId,
                            Text = book.Title,
                            Sent = book.Sent,
                            Pending = book.Pending,
                            IsBusy = IsBusy
                        };

                        authorLibrary.Books.Add(bookLibrary);
                    }

                    authorLibrary.Sent = authorLibrary.Books.All(x => x.Sent && !x.Pending);

                    libraryCollection.Add(authorLibrary);
                }

                var totalPageModule = librarySearchResult.Total % librarySearchResult.PageSize;
                var totalPageCount = librarySearchResult.Total / librarySearchResult.PageSize;
                totalPageCount = totalPageModule > 0 ? totalPageCount + 1 : totalPageCount;

                // If there are no results, set the total count to 1 to avoid division by zero
                TotalPageCount = totalPageCount == 0 ? 1 : totalPageCount;
                CurrentPage = librarySearchResult.Page;
                PaginatorIsVisible = librarySearchResult.Total > librarySearchResult.PageSize;
                TotalCountText = $"{librarySearchResult.Total} Authors, {librarySearchResult.TotalBooks} Books";

                Query = query;
                OnlyUnsent = onlyUnsent;
                Library = libraryCollection;
            }
            catch (Exception ex)
            {
                await ContentPage?.DisplayAlert("Error", ex.Message, "Ok")!;
            }
            finally
            {
                onComplete?.Invoke();
                IsSearching = false;
            }
        }

        private void MarkLibraryAsBusy(bool isBusy)
        {
            foreach (var library in Library ?? [])
                library.IsBusy = isBusy;
        }
    }
}
