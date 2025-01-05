using Eleven.BookManager.App.ViewModels.Base;
using Eleven.BookManager.Business.Contracts;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Eleven.BookManager.App.ViewModels.MainPage
{
    public partial class MainPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private IAmazonService AmazonService { get; set; } = null!;
        private ICalibreService CalibreService { get; set; } = null!;

        public override void UpdateDependencies(IServiceProvider serviceProvider)
        {
            AmazonService = serviceProvider.GetRequiredService<IAmazonService>();
            CalibreService = serviceProvider.GetRequiredService<ICalibreService>();
        }

        private ObservableCollection<LibraryViewModel> library = null!;
        public ObservableCollection<LibraryViewModel> Library
        {
            get { return library; }
            set
            {
                library = value;
                RaisedOnPropertyChanged(nameof(Library));
            }
        }

        private string query = null!;
        public string Query
        {
            get { return query; }
            set { query = value; }
        }

        private bool onlyUnsent;
        public bool OnlyUnsent
        {
            get { return onlyUnsent; }
            set
            {
                onlyUnsent = value;
                RaisedOnPropertyChanged(nameof(onlyUnsent));
            }
        }

        private bool showRecent;
        public bool ShowRecent
        {
            get { return showRecent; }
            set
            {
                showRecent = value;
                RaisedOnPropertyChanged(nameof(ShowRecent));
            }
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                RaisedOnPropertyChanged(nameof(IsBusy));
            }
        }

        private bool isSearching = false;
        public bool IsSearching
        {
            get { return isSearching; }
            set
            {
                isSearching = value;
                RaisedOnPropertyChanged(nameof(IsSearching));
            }
        }

        private bool isSyncing = false;
        public bool IsSyncing
        {
            get { return isSyncing; }
            set
            {
                isSyncing = value;
                RaisedOnPropertyChanged(nameof(IsSyncing));
            }
        }

        private double syncProgress = 0;
        public double SyncProgress
        {
            get { return syncProgress; }
            set
            {
                syncProgress = value;
                RaisedOnPropertyChanged(nameof(SyncProgress));
            }
        }

        private double syncProgressPerc = 0;
        public double SyncProgressPerc
        {
            get { return syncProgressPerc; }
            set
            {
                syncProgressPerc = value;
                RaisedOnPropertyChanged(nameof(SyncProgressPerc));
            }
        }

        private bool paginatorIsVisible;
        public bool PaginatorIsVisible
        {
            get { return paginatorIsVisible; }
            set
            {
                paginatorIsVisible = value;
                RaisedOnPropertyChanged(nameof(PaginatorIsVisible));
            }
        }

        public int PageSize { get; set; } = 10;

        private int currentPage = 1;
        public int CurrentPage
        {
            get { return currentPage; }
            set
            {
                currentPage = value;
                RaisedOnPropertyChanged(nameof(CurrentPage));
            }
        }

        private int totalPageCount = 0;
        public int TotalPageCount
        {
            get { return totalPageCount; }
            set
            {
                totalPageCount = value;
                RaisedOnPropertyChanged(nameof(TotalPageCount));
            }
        }

        private string totalCountText = null!;
        public string TotalCountText
        {
            get { return totalCountText; }
            set
            {
                totalCountText = value;
                RaisedOnPropertyChanged(nameof(TotalCountText));
            }
        }
    }
}
