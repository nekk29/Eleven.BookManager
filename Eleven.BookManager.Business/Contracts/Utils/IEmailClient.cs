using System.Net.Mail;

namespace Eleven.BookManager.Business.Contracts.Utils
{
    public interface IEmailClient
    {
        Task SendEmailAsync(string emailTo, string subject, string body, bool isBodyHtml = false);
        Task SendEmailAsync(string emailTo, string attachment, string subject, string body, bool isBodyHtml = false);
        Task SendEmailAsync(string emailTo, Attachment attachment, string subject, string body, bool isBodyHtml = false);
        Task SendEmailAsync(IEnumerable<string> emailsTo, IEnumerable<string> emailsCC, IEnumerable<string> fileAttachments, IEnumerable<Attachment> attachments, string subject, string body, bool isBodyHtml = false);
    }
}
