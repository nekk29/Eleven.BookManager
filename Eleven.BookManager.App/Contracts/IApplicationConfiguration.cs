using Eleven.BookManager.Business.Models.Configuration;

namespace Eleven.BookManager.App.Contracts
{
    public interface IApplicationConfiguration
    {
        AppConfiguration LoadAppConfiguration();
        AppConfiguration GetAppConfiguration();
        AppConfiguration SaveAppConfiguration(AppConfiguration configuration);
    }
}
