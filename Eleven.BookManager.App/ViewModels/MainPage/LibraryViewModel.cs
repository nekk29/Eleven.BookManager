using Eleven.BookManager.App.ViewModels.Base;
using Eleven.BookManager.Business.Extensions;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Eleven.BookManager.App.ViewModels.MainPage
{
    public partial class LibraryViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly string ALPHABET = "abcdefghijklmnopqrstuvwxyz";

        public Guid? AuthorId { get; set; } = null!;
        public Guid? AuthorBookId { get; set; } = null!;
        public string Text { get; set; } = null!;
        public bool Sent { get; set; } = false;
        public bool Pending { get; set; } = false;


        public bool IsAuthor => AuthorId.HasValue;
        public bool IsBook => AuthorBookId.HasValue;

        public string RowColor => IsAuthor ? "Transparent" : "#333333";
        public ImageSource ImageIcon => IsAuthor ? "author.png" : "book.png";

        public bool MarkPendingIsVisible => IsBook && !Sent;
        public bool MarkPendingIsEnabled => !Sent && !IsBusy;
        public string MarkPendingText => Pending ? "Mark As Complete" : "Mark As Pending";

        public ImageSource SentIcon => Sent ? "checked.png" : (Pending ? "warning.png" : "exclamation.png");

        public static bool SendIsVisible => true;
        public bool SendIsEnabled => !Sent && !IsBusy;

        public static bool MarkSentIsVisible => true;
        public bool MarkSentIsEnabled => !IsBusy;
        public string MarkSentText => Sent ? "Mark As Unsent" : "Mark As Sent";

        public static bool DeleteIsVisible => true;
        public bool DeleteIsEnabled => IsBook && !Sent && !IsBusy;
        public string DeleteText => IsBook ? "Delete" : "";

        public ObservableCollection<LibraryViewModel> Books { get; set; } = [];

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                RaisedOnPropertyChanged(nameof(IsBusy));
                RefreshBusyProperties();
                foreach (var book in Books ?? []) book.IsBusy = value;
            }
        }

        private void RefreshBusyProperties()
        {
            RaisedOnPropertyChanged(nameof(MarkPendingIsEnabled));
            RaisedOnPropertyChanged(nameof(SendIsEnabled));
            RaisedOnPropertyChanged(nameof(MarkSentIsEnabled));
            RaisedOnPropertyChanged(nameof(DeleteIsEnabled));
        }

        private bool isSendingAuthor = false;
        public bool IsSendingAuthor
        {
            get { return isSendingAuthor; }
            set
            {
                isSendingAuthor = value;
                RaisedOnPropertyChanged(nameof(IsSendingAuthor));
            }
        }

        private bool isSendingBook = false;
        public bool IsSendingBook
        {
            get { return isSendingBook; }
            set
            {
                isSendingBook = value;
                RaisedOnPropertyChanged(nameof(IsSendingBook));
            }
        }

        private double sendProgress = 0;
        public double SendProgress
        {
            get { return sendProgress; }
            set
            {
                sendProgress = value;
                RaisedOnPropertyChanged(nameof(SendProgress));
            }
        }

        private double sendProgressPerc = 0;
        public double SendProgressPerc
        {
            get { return sendProgressPerc; }
            set
            {
                sendProgressPerc = value;
                RaisedOnPropertyChanged(nameof(SendProgressPerc));
            }
        }

        private string sendProgressText = null!;
        public string SendProgressText
        {
            get { return sendProgressText; }
            set
            {
                sendProgressText = value;
                RaisedOnPropertyChanged(nameof(SendProgressText));
            }
        }

        public bool LetterIconIsVisible { get; set; }
        public ImageSource LetterIcon { get; set; } = "letter__symbol.png";

        public void UpdateLetterIcon()
        {
            if (IsBook) return;

            var firstLetter = GetFirstLetter();

            if (string.IsNullOrEmpty(firstLetter))
            {
                LetterIcon = "letter__symbol.png";
                return;
            }

            LetterIcon = $"letter_{firstLetter}.png";
        }

        public string GetFirstLetter()
        {
            if (Text.Length == 0) return null!;

            var letter = Text.Clear().ToLower()[0];

            return ALPHABET.Contains(letter) ? letter.ToString() : null!;
        }
    }
}
