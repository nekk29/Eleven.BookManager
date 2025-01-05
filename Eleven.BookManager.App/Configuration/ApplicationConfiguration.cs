using Eleven.BookManager.App.Contracts;
using Eleven.BookManager.Business.Contracts.Utils;
using Eleven.BookManager.Business.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Eleven.BookManager.App.Configuration
{
    public class ApplicationConfiguration(
        IEncrypter encrypter,
        IConfigurationManager configurationManager
    ) : IApplicationConfiguration
    {
        private readonly IEncrypter _encrypter = encrypter;
        private readonly IConfigurationManager _configurationManager = configurationManager;

        public AppConfiguration LoadAppConfiguration()
        {
            try
            {
                var appSettingsFile = GetAppSettingsFile();
                var appConfiguration = GetAppConfiguration();

                if (appConfiguration != null)
                    _configurationManager.AddJsonFile(appSettingsFile, false, true);

                return appConfiguration!;
            }
            catch
            {
                return null!;
            }
        }

        public AppConfiguration GetAppConfiguration()
        {
            var appSettingsFile = GetAppSettingsFile();

            if (!File.Exists(appSettingsFile))
                return CreateAppConfiguration();

            try
            {
                var appSettingsFileContent = File.ReadAllText(appSettingsFile);
                var appConfiguration = JsonConvert.DeserializeObject<AppConfiguration>(appSettingsFileContent);

                if (!string.IsNullOrEmpty(appConfiguration?.SmtpOptions?.Password))
                {
                    try
                    {
                        appConfiguration.SmtpOptions.Password = _encrypter.Decrypt(
                            appConfiguration.AppOptions.AppId,
                            appConfiguration.SmtpOptions.Password!
                        );
                    }
                    catch
                    {
                        appConfiguration.SmtpOptions.Password = null!;
                    }
                }

                return appConfiguration!;
            }
            catch
            {
                return CreateAppConfiguration();
            }
        }

        public void UpdateWorkingDirectory(string workingDirectory)
        {
            var appConfiguration = GetAppConfiguration();

            appConfiguration.AppOptions ??= new AppOptions();
            appConfiguration.AppOptions.IsConfigured = true;
            appConfiguration.AppOptions.WorkingDirectory = workingDirectory;

            SaveAppConfiguration(appConfiguration);
        }

        public AppConfiguration SaveAppConfiguration(AppConfiguration appConfiguration)
        {
            if (string.IsNullOrEmpty(appConfiguration.AppOptions.AppId))
                appConfiguration.AppOptions.AppId = Guid.NewGuid().ToString();

            appConfiguration.AppOptions.IsConfigured =
                Directory.Exists(appConfiguration.AppOptions.WorkingDirectory) &&
                Directory.Exists(appConfiguration.CalibreOptions.LibraryDirectory);

            if (!string.IsNullOrEmpty(appConfiguration.SmtpOptions.Password))
            {
                appConfiguration.SmtpOptions.Password = _encrypter.Encrypt(
                    appConfiguration.AppOptions.AppId,
                    appConfiguration.SmtpOptions.Password!
                );
            }

            var appSettingsFile = GetAppSettingsFile();
            var appSettingsFileContent = JsonConvert.SerializeObject(appConfiguration, Formatting.Indented);

            File.WriteAllText(appSettingsFile, appSettingsFileContent);

            return appConfiguration;
        }

        private AppConfiguration CreateAppConfiguration()
        {
            var appId = Guid.NewGuid().ToString();

            var appConfiguration = new AppConfiguration
            {
                AppOptions = new AppOptions { AppId = appId },
                AmazonOptions = new AmazonOptions(),
                CalibreOptions = new CalibreOptions(),
                SmtpOptions = new SmtpOptions(),
            };

            SaveAppConfiguration(appConfiguration);

            return appConfiguration;
        }

        private static string GetAppSettingsFile()
        {
            var appSettingsDirectory = GetOrCreateAppSettingsDirectory();
            var appSettingsFile = Path.Combine(appSettingsDirectory, "appsettings.json");
            return appSettingsFile;
        }

        private static string GetOrCreateAppSettingsDirectory()
        {
            var appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appSettingsDirectory = Path.Combine(appDataDirectory, "Eleven");

            if (!Directory.Exists(appSettingsDirectory))
                Directory.CreateDirectory(appSettingsDirectory);

            appSettingsDirectory = Path.Combine(appSettingsDirectory, "BookManager");

            if (!Directory.Exists(appSettingsDirectory))
                Directory.CreateDirectory(appSettingsDirectory);

            return appSettingsDirectory;
        }
    }
}
