using Eleven.BookManager.App.ViewModels.Base;

namespace Eleven.BookManager.App.ViewModels.Settings
{
    public partial class AmazonOptionsViewModel : ViewModelBase
    {
        private string accountEmail = null!;
        public string AccountEmail
        {
            get { return accountEmail; }
            set
            {
                accountEmail = value;
                RaisedOnPropertyChanged(nameof(AccountEmail));
            }
        }
    }
}
