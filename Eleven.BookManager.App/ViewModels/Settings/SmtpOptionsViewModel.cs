using Eleven.BookManager.App.ViewModels.Base;

namespace Eleven.BookManager.App.ViewModels.Settings
{
    public partial class SmtpOptionsViewModel : ViewModelBase
    {
        private string server = null!;
        public string Server
        {
            get { return server; }
            set
            {
                server = value;
                RaisedOnPropertyChanged(nameof(Server));
            }
        }

        private string port = null!;
        public string Port
        {
            get { return port; }
            set
            {
                port = value;
                RaisedOnPropertyChanged(nameof(Port));
            }
        }

        private string fromDisplayName = null!;
        public string FromDisplayName
        {
            get { return fromDisplayName; }
            set
            {
                fromDisplayName = value;
                RaisedOnPropertyChanged(nameof(FromDisplayName));
            }
        }

        private string email = null!;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                RaisedOnPropertyChanged(nameof(Email));
            }
        }

        private string password = null!;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                RaisedOnPropertyChanged(nameof(Password));
            }
        }

        private string confirmPassword = null!;
        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set
            {
                confirmPassword = value;
                RaisedOnPropertyChanged(nameof(ConfirmPassword));
            }
        }
    }
}
