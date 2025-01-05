using Eleven.BookManager.App.ViewModels.Base;

namespace Eleven.BookManager.App.ViewModels.Settings
{
    public partial class CalibreOptionsViewModel : ViewModelBase
    {
        private string libraryDirectory = null!;
        public string LibraryDirectory
        {
            get { return libraryDirectory; }
            set
            {
                libraryDirectory = value;
                RaisedOnPropertyChanged(nameof(LibraryDirectory));
            }
        }
    }
}
