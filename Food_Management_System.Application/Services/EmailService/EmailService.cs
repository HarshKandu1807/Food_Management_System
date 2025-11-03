using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.EmailService
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmail(
            string toEmail,
            string subject,
            string body,
            byte[]? attachmentBytes = null,
            string? attachmentName = null,
            string? contentType = null)
        {
            var fromEmail = _config["EmailSettings:FromEmail"];
            var encodedPassword = _config["EmailSettings:Password"];
            var smtpHost = _config["EmailSettings:Host"];
            var port = int.Parse(_config["EmailSettings:Port"] ?? "587");

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(fromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };

            if (attachmentBytes != null && !string.IsNullOrEmpty(attachmentName))
            {
                builder.Attachments.Add(
                    attachmentName,
                    attachmentBytes,
                    new ContentType("application", contentType ?? "octet-stream")
                );
            }

            message.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(smtpHost, port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(fromEmail, Encoding.UTF8.GetString(Convert.FromBase64String(encodedPassword!)));
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

    }
}
