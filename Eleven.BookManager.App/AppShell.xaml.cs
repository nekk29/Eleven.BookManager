using Eleven.BookManager.App.Contracts;

namespace Eleven.BookManager.App
{
    public partial class AppShell : Shell
    {
        private bool IsConfigured { get; set; } = false;

        private readonly IApplicationConfiguration _configuration;

        public AppShell(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _configuration = serviceProvider.GetRequiredService<IApplicationConfiguration>();

            InitializeSettings();
        }

        private void InitializeSettings()
        {
            var appConfiguration = _configuration.LoadAppConfiguration();

            IsConfigured = appConfiguration?.AppOptions?.IsConfigured == true;

            if (!IsConfigured)
            {
                ShowMainMenu(false);
                GoToAsync("//SettingsPage").Wait();
            }
        }

        public void ShowMainMenu(bool show)
        {
            SettingsPageContent.IsVisible = false;
            MainPageContent.IsVisible = show;
            SettingsPageContent.IsVisible = true;
        }
    }
}
