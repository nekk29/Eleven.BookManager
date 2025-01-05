using Eleven.BookManager.Business.Contracts.Configuration;
using Eleven.BookManager.Repository.Contracts;

namespace Eleven.BookManager.Business.Configuration
{
    public class RepositoryConfiguration(IBusinessConfiguration configuration) : IRepositoryConfiguration
    {
        private readonly IBusinessConfiguration _configuration = configuration;

        public string GetConnectionString()
        {
            var appOptions = _configuration.GetAppOptions();
            if (appOptions == null) return null!;

            var connectionString = Path.Combine(appOptions.WorkingDirectory, "library.db");

            return $"Data Source={connectionString}";
        }
    }
}
