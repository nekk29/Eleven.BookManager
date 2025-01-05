using System.ComponentModel;

namespace Eleven.BookManager.App.ViewModels.Base
{
    public partial class ViewModelBase : INotifyPropertyChanged
    {
        protected ContentPage ContentPage { get; set; } = null!;

        public event PropertyChangedEventHandler? PropertyChanged;
        public void RaisedOnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void UpdateContentPage(ContentPage contentPage) => ContentPage = contentPage;

        public virtual void UpdateDependencies(IServiceProvider serviceProvider) { }

        protected void Execute(Func<Task> func, Action onStart = null!, Action onEnd = null!)
        {
            Task.Run(async () =>
            {
                onStart?.Invoke();
                ContentPage?.BatchBegin();
                await func();
                ContentPage?.BatchCommit();
                onEnd?.Invoke();
            });
        }
    }
}
