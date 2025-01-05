using Eleven.BookManager.App.ViewModels.Base;

namespace Eleven.BookManager.App.ViewModels.Settings
{
    public partial class AppOptionsViewModel : ViewModelBase
    {
        private bool isConfigured;
        public bool IsConfigured
        {
            get { return isConfigured; }
            set
            {
                isConfigured = value;
                RaisedOnPropertyChanged(nameof(IsConfigured));
            }
        }

        private string workingDirectory = null!;
        public string WorkingDirectory
        {
            get { return workingDirectory; }
            set
            {
                workingDirectory = value;
                RaisedOnPropertyChanged(nameof(WorkingDirectory));
            }
        }
    }
}
