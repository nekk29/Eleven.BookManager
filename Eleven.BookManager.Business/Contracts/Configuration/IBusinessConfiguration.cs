using Eleven.BookManager.Business.Models.Configuration;

namespace Eleven.BookManager.Business.Contracts.Configuration
{
    public interface IBusinessConfiguration
    {
        AppOptions GetAppOptions();
        AmazonOptions GetAmazonOptions();
        CalibreOptions GetCalibreOptions();
        SmtpOptions GetSmtpOptions();
    }
}
