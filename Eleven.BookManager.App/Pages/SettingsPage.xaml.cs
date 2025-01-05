using CommunityToolkit.Maui.Storage;
using Eleven.BookManager.App.Contracts;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Eleven.BookManager.App.Pages
{
    public partial class SettingsPage : ContentPage
    {
        private readonly IApplicationConfiguration _configuration;

        public SettingsPage(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            InitializeComponents(serviceProvider);

            _configuration = serviceProvider.GetRequiredService<IApplicationConfiguration>();
        }

        private void InitializeComponents(IServiceProvider serviceProvider)
        {
            ViewModel.UpdateDependencies(serviceProvider);
            ViewModel.UpdateContentPage(this);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.Current.Navigating += OnNavigating!;
        }

        protected override void OnDisappearing()
        {
            Shell.Current.Navigating -= OnNavigating!;
            base.OnDisappearing();
        }

        private void OnNavigating(object sender, ShellNavigatingEventArgs args)
        {
            var appConfiguration = _configuration.LoadAppConfiguration();
            if (appConfiguration?.AppOptions?.IsConfigured == false) args.Cancel();
        }

        private void ContentPage_Loaded(object sender, EventArgs e) => ViewModel.LoadConfiguration();

        private async void SelectDataLocationButton_Clicked(object sender, EventArgs e)
        {
            var source = new CancellationTokenSource();
            var result = await FolderPicker.Default.PickAsync(source.Token);

            if (result.IsSuccessful)
                ViewModel.AppOptions.WorkingDirectory = result.Folder.Path;
        }

        private async void SelectCalibreLocationButton_Clicked(object sender, EventArgs e)
        {
            var source = new CancellationTokenSource();
            var result = await FolderPicker.Default.PickAsync(source.Token);

            if (result.IsSuccessful)
                ViewModel.CalibreOptions.LibraryDirectory = result.Folder.Path;
        }

        private void PortEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var regex = e.NewTextValue;

            if (string.IsNullOrEmpty(regex)) return;

            if (!Regex.Match(regex, "^[0-9]+$").Success)
            {
                var entry = sender as Entry;
                if (entry != null)
                    entry.Text = (string.IsNullOrEmpty(e.OldTextValue)) ? string.Empty : e.OldTextValue;
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (ValidateFormData())
            {
                if (!Directory.Exists(ViewModel.AppOptions.WorkingDirectory))
                {
                    await DisplayAlert("Warning", "¡ Selected folder for application data does not exist ! ", "Ok")!;
                    return;
                }

                if (!Directory.Exists(ViewModel.CalibreOptions.LibraryDirectory))
                {
                    await DisplayAlert("Warning", "¡ Selected folder for calibre library does not exist ! ", "Ok")!;
                    return;
                }

                if (ViewModel.AppOptions.IsConfigured)
                {
                    if (!IsValidEmail(ViewModel.AmazonOptions.AccountEmail))
                    {
                        await DisplayAlert("Warning", "¡ Invalid Amazon email address ! ", "Ok")!;
                        return;
                    }

                    if (!IsValidEmail(ViewModel.SmtpOptions.Email))
                    {
                        await DisplayAlert("Warning", "¡ Invalid SMTP email address ! ", "Ok")!;
                        return;
                    }

                    var portParsed = int.TryParse(ViewModel.SmtpOptions.Port, out int port);
                    if (!portParsed && (port < 0 || port > 65535))
                    {
                        await DisplayAlert("Warning", "¡ Invalid port number ! ", "Ok")!;
                        return;
                    }

                    if (ViewModel.SmtpOptions.Password != ViewModel.SmtpOptions.ConfirmPassword)
                    {
                        await DisplayAlert("Warning", "¡ Passwords do not match ! ", "Ok")!;
                        return;
                    }
                }

                try
                {
                    var result = ViewModel.SaveConfiguration();

                    if (result.AppOptions.IsConfigured)
                    {
                        ViewModel.AppOptions.IsConfigured = result.AppOptions.IsConfigured;

                        await DisplayAlert("Result", "¡ App configuration was saved successfully ! ", "Ok")!;

                        if (Shell.Current is AppShell appShell) appShell.ShowMainMenu(true);
                    }
                }
                catch
                {
                    await DisplayAlert("Error", "¡ There was an error saving the configuration ! ", "Ok")!;
                }
            }
            else
            {
                await DisplayAlert("Warning", "¡ Please fill the missing fields ! ", "Ok")!;
            }
        }

        private bool ValidateFormData()
        {
            var validEntries = new List<bool>();

            if (ViewModel.AppOptions?.IsConfigured == false)
            {
                validEntries.Add(ValidateAndMarkEntry(ViewModel.AppOptions?.WorkingDirectory!, AppOptionsWorkingDirectoryEntry));
                validEntries.Add(ValidateAndMarkEntry(ViewModel.CalibreOptions?.LibraryDirectory!, CalibreOptionsLibraryDirectoryEntry));

                return validEntries.All(x => x);
            }

            validEntries.Add(ValidateAndMarkEntry(ViewModel.AmazonOptions?.AccountEmail!, AmazonOptionsAccountEmailEntry));
            validEntries.Add(ValidateAndMarkEntry(ViewModel.SmtpOptions?.Server!, SmtpOptionsServerEntry));
            validEntries.Add(ValidateAndMarkEntry(ViewModel.SmtpOptions?.Port!, SmtpOptionsPortEntry));
            validEntries.Add(ValidateAndMarkEntry(ViewModel.SmtpOptions?.FromDisplayName!, SmtpOptionsFromDisplayNameEntry));
            validEntries.Add(ValidateAndMarkEntry(ViewModel.SmtpOptions?.Email!, SmtpOptionsEmailEntry));
            validEntries.Add(ValidateAndMarkEntry(ViewModel.SmtpOptions?.Password!, SmtpOptionsPasswordEntry));
            validEntries.Add(ValidateAndMarkEntry(ViewModel.SmtpOptions?.ConfirmPassword!, SmtpOptionsConfirmPasswordEntry));

            return validEntries.All(x => x);
        }

        private bool ValidateAndMarkEntry(string value, Entry entry)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (Resources.TryGetValue("EntryInvalid", out object entryInvalid))
                    if (entryInvalid is Style style) entry.Style = style;
                return false;
            }

            if (Resources.TryGetValue("EntryValid", out object entryValid))
                if (entryValid is Style style) entry.Style = style;

            return true;
        }

        private static bool IsValidEmail(string email)
        {
            try { var emailAddress = new MailAddress(email); return true; }
            catch { return false; }
        }
    }
}
