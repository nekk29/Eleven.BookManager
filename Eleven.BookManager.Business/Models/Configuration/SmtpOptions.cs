namespace Eleven.BookManager.Business.Models.Configuration
{
    public class SmtpOptions
    {
        public string? Server { get; set; }
        public int Port { get; set; }
        public string? From { get; set; }
        public string? FromDisplayName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
