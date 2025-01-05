namespace Eleven.BookManager.Business.Models.Configuration
{
    public class AppOptions
    {
        public string AppId { get; set; } = null!;
        public string WorkingDirectory { get; set; } = null!;
        public bool IsConfigured { get; set; }

    }
}
