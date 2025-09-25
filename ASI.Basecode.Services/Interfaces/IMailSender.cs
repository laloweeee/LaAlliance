using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IMailSender
    {
        Task SendWelcomeEmailAsync(string toEmail, string userName);
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
        Task SendEmailVerificationAsync(string toEmail, int OtpCode);
        Task sendCustomEmailAsync(string toEmail, string subject, string body);
    }
}
