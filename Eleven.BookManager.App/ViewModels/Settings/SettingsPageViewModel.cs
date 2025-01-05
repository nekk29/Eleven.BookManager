using Eleven.BookManager.App.Contracts;
using Eleven.BookManager.App.ViewModels.Base;

namespace Eleven.BookManager.App.ViewModels.Settings
{
    public partial class SettingsPageViewModel : ViewModelBase
    {
        private IApplicationConfiguration Configuration { get; set; } = null!;

        public override void UpdateDependencies(IServiceProvider serviceProvider)
        {
            Configuration = serviceProvider.GetRequiredService<IApplicationConfiguration>();
        }

        private AppOptionsViewModel appOptions = null!;
        public AppOptionsViewModel AppOptions
        {
            get { return appOptions; }
            set
            {
                appOptions = value;
                RaisedOnPropertyChanged(nameof(AppOptions));
            }
        }

        private CalibreOptionsViewModel calibreOptions = null!;
        public CalibreOptionsViewModel CalibreOptions
        {
            get { return calibreOptions; }
            set
            {
                calibreOptions = value;
                RaisedOnPropertyChanged(nameof(CalibreOptions));
            }
        }

        private AmazonOptionsViewModel amazonOptions = null!;
        public AmazonOptionsViewModel AmazonOptions
        {
            get { return amazonOptions; }
            set
            {
                amazonOptions = value;
                RaisedOnPropertyChanged(nameof(AmazonOptions));
            }
        }

        private SmtpOptionsViewModel smtpOptions = null!;
        public SmtpOptionsViewModel SmtpOptions
        {
            get { return smtpOptions; }
            set
            {
                smtpOptions = value;
                RaisedOnPropertyChanged(nameof(SmtpOptions));
            }
        }
    }
}
