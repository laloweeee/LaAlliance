using ASI.Basecode.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    /// <summary>
    /// Service for sending emails such as welcome emails, password resets, and email verifications.
    /// </summary>
    public class MailSenderService : IMailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MailSenderService> _logger;

        public MailSenderService(IConfiguration configuration, ILogger<MailSenderService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Sends a welcome email to a new user.
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            try
            {
                _logger.LogInformation($"Sending welcome email to {toEmail}");
                var subject = "Welcome to LaAllianc�!";
                var htmlContent = $"Welcome to LaAllianc� {userName}";

                await sendCustomEmailAsync(toEmail, subject, htmlContent);
                _logger.LogInformation($"Welcome email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send welcome email to {toEmail}");
                throw;
            }
        }

        /// <summary>
        /// Sends a password reset email with an OTP code.
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="otpCode"></param>
        /// <returns></returns>
        public async Task SendPasswordResetEmailAsync(string toEmail, string otpCode)
        {
            try
            {
                var subject = "Reset Your Password - LaAllianc�";
                var htmlContent = $"Your OTP Code: {otpCode}";

                await sendCustomEmailAsync(toEmail, subject, htmlContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send password reset email to {toEmail}");
                throw;
            }
        }

        /// <summary>
        /// Sends an email verification email with an OTP code.
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="otpCode"></param>
        /// <returns></returns>
        public async Task SendEmailVerificationAsync(string toEmail, int otpCode)
        {
            try
            {
                var subject = "Verify Your Email - LaAllianc�";
                var htmlContent = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h2 style='color: #4CAF50;'>Verify Your Email Address</h2>
                        <p>Thank you for registering with LaAllianc�! To complete your registration, please use the verification code below:</p>
                        <div style='background-color: #f2f2f2; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 5px; margin: 20px 0;'>
                            {otpCode}
                        </div>
                        <p>This code will expire in 10 minutes.</p>
                        <p>If you didn't create an account with LaAllianc�, you can safely ignore this email.</p>
                        <p>Thank you,<br>The LaAllianc� Team</p>
                    </div>";

                await sendCustomEmailAsync(toEmail, subject, htmlContent);
                _logger.LogInformation($"Verification email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email verification to {toEmail}");
                throw;
            }
        }

        /// <summary>
        /// Sends a custom email with specified subject and HTML content.
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="subject"></param>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        public async Task sendCustomEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var smtpServer = _configuration["Email:Host"];
            var smtpPort = int.Parse(_configuration["Email:Port"]);
            var fromEmail = _configuration["Email:FromEmail"];
            var fromPassword = _configuration["Email:Password"];

            var client = new SmtpClient("live.smtp.mailtrap.io", 587)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_configuration["Email:Username"], fromPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}