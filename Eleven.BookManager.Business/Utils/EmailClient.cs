using Eleven.BookManager.Business.Contracts.Configuration;
using Eleven.BookManager.Business.Contracts.Utils;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Eleven.BookManager.Business.Utils
{
    public class EmailClient(IBusinessConfiguration configuration) : IEmailClient
    {
        private readonly IBusinessConfiguration _configuration = configuration;

        public async Task SendEmailAsync(string emailTo, string subject, string body, bool isBodyHtml = false)
            => await SendEmailAsync([emailTo], null, null, null, subject, body, isBodyHtml);

        public async Task SendEmailAsync(string emailTo, string attachment, string subject, string body, bool isBodyHtml = false)
            => await SendEmailAsync([emailTo], null, [attachment], null, subject, body, isBodyHtml);

        public async Task SendEmailAsync(string emailTo, Attachment attachment, string subject, string body, bool isBodyHtml = false)
            => await SendEmailAsync([emailTo], null, null, [attachment], subject, body, isBodyHtml);

        public async Task SendEmailAsync(IEnumerable<string> emailsTo, IEnumerable<string>? emailsCC, IEnumerable<string>? fileAttachments, IEnumerable<Attachment>? attachments, string subject, string body, bool isBodyHtml = false)
        {
            var options = _configuration.GetSmtpOptions() ?? throw new Exception("Options for email not found");

            if (emailsTo == null)
                throw new Exception("To email list cannot be null");

            if (!emailsTo.Any())
                throw new Exception("To email list cannot be empty");

            if (string.IsNullOrEmpty(subject))
                throw new Exception("The subject cannot be empty");

            if (string.IsNullOrEmpty(body))
                throw new Exception("The body cannot be empty");

            using var mailMessage = new MailMessage();

            foreach (var emailTo in emailsTo)
                mailMessage.To.Add(emailTo);

            if (emailsCC != null)
            {
                foreach (var emailCC in emailsCC)
                    mailMessage.CC.Add(emailCC);
            }

            if (fileAttachments != null)
            {
                foreach (var attachment in fileAttachments)
                    mailMessage.Attachments.Add(new Attachment(attachment));
            }

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                    mailMessage.Attachments.Add(attachment);
            }

            var from = string.IsNullOrWhiteSpace(options.From) ? options.Email : options.From;
            var displayName = string.IsNullOrWhiteSpace(options.FromDisplayName) ? from : options.FromDisplayName;

            mailMessage.From = new MailAddress(from ?? string.Empty, displayName, Encoding.UTF8);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = isBodyHtml;

            using var smtpClient = new SmtpClient(options.Server, options.Port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            smtpClient.Credentials = new NetworkCredential(options.Email, options.Password);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
