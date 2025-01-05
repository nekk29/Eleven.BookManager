using Eleven.BookManager.Business.Contracts.Configuration;
using Eleven.BookManager.Business.Contracts.Utils;
using Eleven.BookManager.Business.Models.Configuration;
using Microsoft.Extensions.Configuration;

namespace Eleven.BookManager.Business.Configuration
{
    public class BusinessConfiguration(
        IEncrypter encrypter,
        IConfiguration configuration
    ) : IBusinessConfiguration
    {
        private readonly IEncrypter _encrypter = encrypter;
        private readonly IConfiguration _configuration = configuration;

        public AppOptions GetAppOptions()
            => _configuration.GetSection("AppOptions").Get<AppOptions>()!;

        public AmazonOptions GetAmazonOptions()
            => _configuration.GetSection("AmazonOptions").Get<AmazonOptions>()!;

        public CalibreOptions GetCalibreOptions()
            => _configuration.GetSection("CalibreOptions").Get<CalibreOptions>()!;

        public SmtpOptions GetSmtpOptions()
        {
            var appOptions = GetAppOptions();
            var smtpOptions = _configuration.GetSection("SmtpOptions").Get<SmtpOptions>()!;

            if (!string.IsNullOrEmpty(smtpOptions.Password))
            {
                try
                {
                    smtpOptions.Password = _encrypter.Decrypt(
                        appOptions.AppId,
                        smtpOptions.Password!
                    );
                }
                catch
                {
                    smtpOptions.Password = null!;
                }
            }

            return smtpOptions;
        }
    }
}
