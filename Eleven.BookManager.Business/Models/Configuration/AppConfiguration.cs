namespace Eleven.BookManager.Business.Models.Configuration
{
    public class AppConfiguration
    {
        public AppOptions AppOptions { get; set; } = null!;
        public CalibreOptions CalibreOptions { get; set; } = null!;
        public AmazonOptions AmazonOptions { get; set; } = null!;
        public SmtpOptions SmtpOptions { get; set; } = null!;
    }
}
