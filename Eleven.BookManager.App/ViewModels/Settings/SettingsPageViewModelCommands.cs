using Eleven.BookManager.App.ViewModels.Base;
using Eleven.BookManager.Business.Models.Configuration;
using System.ComponentModel;

namespace Eleven.BookManager.App.ViewModels.Settings
{
    public partial class SettingsPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public void LoadConfiguration()
        {
            var configuration = Configuration.GetAppConfiguration();

            AppOptions = new AppOptionsViewModel
            {
                IsConfigured = configuration.AppOptions?.IsConfigured == true,
                WorkingDirectory = configuration.AppOptions?.WorkingDirectory!
            };

            CalibreOptions = new CalibreOptionsViewModel
            {
                LibraryDirectory = configuration.CalibreOptions?.LibraryDirectory!
            };

            AmazonOptions = new AmazonOptionsViewModel
            {
                AccountEmail = configuration.AmazonOptions?.AccountEmail!
            };

            var port = configuration.SmtpOptions?.Port == default(int)
                ? null : configuration.SmtpOptions?.Port.ToString();

            SmtpOptions = new SmtpOptionsViewModel
            {
                Server = configuration.SmtpOptions?.Server!,
                Port = port!,
                FromDisplayName = configuration.SmtpOptions?.FromDisplayName!,
                Email = configuration.SmtpOptions?.Email!,
                Password = configuration.SmtpOptions?.Password!,
                ConfirmPassword = configuration.SmtpOptions?.Password!
            };
        }

        public AppConfiguration SaveConfiguration()
        {
            var configuration = Configuration.GetAppConfiguration();

            configuration.AppOptions = new AppOptions
            {
                WorkingDirectory = AppOptions.WorkingDirectory
            };

            configuration.CalibreOptions = new CalibreOptions
            {
                LibraryDirectory = CalibreOptions.LibraryDirectory
            };

            configuration.AmazonOptions = new AmazonOptions
            {
                AccountEmail = AmazonOptions.AccountEmail
            };

            var parsed = int.TryParse(SmtpOptions.Port, out var port);

            configuration.SmtpOptions = new SmtpOptions
            {
                Server = SmtpOptions.Server,
                Port = parsed ? port : 0,
                From = SmtpOptions.Email,
                FromDisplayName = SmtpOptions.FromDisplayName,
                Email = SmtpOptions.Email,
                Password = SmtpOptions.Password
            };

            return Configuration.SaveAppConfiguration(configuration);
        }
    }
}
