using CommunityToolkit.Maui;
using Eleven.BookManager.App.Configuration;
using Eleven.BookManager.App.Contracts;
using Eleven.BookManager.App.Services;
using Eleven.BookManager.Business;
using Eleven.BookManager.Business.Configuration;
using Eleven.BookManager.Business.Contracts;
using Eleven.BookManager.Business.Contracts.Configuration;
using Eleven.BookManager.Business.Contracts.Utils;
using Eleven.BookManager.Business.Utils;
using Eleven.BookManager.Repository;
using Eleven.BookManager.Repository.Contracts;
using Eleven.BookManager.Repository.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Eleven.BookManager.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp
                .CreateBuilder()
                .UseMauiApp(CreateApp)
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            ConfigureServices(builder.Services, builder.Configuration);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        static App CreateApp(IServiceProvider serviceProvider) => new(serviceProvider);

        static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddScoped<IConfigurationManager>((_) => configuration);

            services.AddScoped<IEncrypter, Encrypter>();
            services.AddScoped<IEpubReader, EpubReader>();
            services.AddScoped<IEmailClient, EmailClient>();
            services.AddScoped<IUserIdentity, UserIdentity>();
            services.AddScoped<IAmazonService, AmazonService>();
            services.AddScoped<ICalibreService, CalibreService>();
            services.AddScoped<IBusinessConfiguration, BusinessConfiguration>();
            services.AddScoped<IRepositoryConfiguration, RepositoryConfiguration>();
            services.AddScoped<IApplicationConfiguration, ApplicationConfiguration>();

            services.AddScoped<CoreDbContext>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}
