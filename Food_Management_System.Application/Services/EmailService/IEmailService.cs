using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Food_Management_System.Application.Services.EmailService
{
    public interface IEmailService
    {
        Task SendEmail(
        string toEmail,
        string subject,
        string body,
        byte[]? attachmentBytes = null,
        string? attachmentName = null,
        string? contentType = null);
    }
}
