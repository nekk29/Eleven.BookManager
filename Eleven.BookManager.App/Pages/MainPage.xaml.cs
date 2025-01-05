using Eleven.BookManager.App.Extensions;
using Eleven.BookManager.App.Utils;
using Controls = UraniumUI.Material.Controls;

namespace Eleven.BookManager.App.Pages
{
    public partial class MainPage : ContentPage
    {
        private readonly Debouncer _debouncer = new();

        public MainPage(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            InitializeComponents(serviceProvider);
        }

        private void InitializeComponents(IServiceProvider serviceProvider)
        {
            LibraryTree.ChildrenBinding = new Binding("Books");
            ViewModel.UpdateDependencies(serviceProvider);
            ViewModel.UpdateContentPage(this);
        }

        private void ContentPage_Loaded(object sender, EventArgs e) => this.Execute(LoadLibrary);

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            await _debouncer.Debounce(async () =>
            {
                ViewModel.CurrentPage = 1;

                await ViewModel.SearchLibrary(SearchBar.Text, ViewModel.OnlyUnsent, ViewModel.ShowRecent, () =>
                {
                    ResetTreeStatus();
                });
            }, 500);
        }

        private async void OnlyUnsent_Tapped(object sender, TappedEventArgs e)
        {
            ViewModel.CurrentPage = 1;

            await ViewModel.SearchLibrary(ViewModel.Query, OnlyUnsentCheckBox.IsChecked, ViewModel.ShowRecent, () =>
            {
                ResetTreeStatus();
            });
        }

        private async void ShowRecent_Tapped(object sender, TappedEventArgs e)
        {
            ViewModel.CurrentPage = 1;

            await ViewModel.SearchLibrary(ViewModel.Query, ViewModel.OnlyUnsent, ShowRecentCheckBox.IsChecked, () =>
            {
                ResetTreeStatus();
            });
        }

        public void ResetTreeStatus()
        {
            foreach (var child in LibraryTree.Children ?? [])
            {
                if (child is Controls.TreeViewNodeHolderView holderView)
                    holderView.IsExpanded = false;
            }
        }

        private async Task LoadLibrary() => await ViewModel.SearchLibrary(ViewModel.Query, ViewModel.OnlyUnsent, ViewModel.ShowRecent);
    }
}
