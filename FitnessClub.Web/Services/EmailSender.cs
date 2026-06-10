using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace FitnessClub.API.Services
{
    // Settings voor e-mail
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
        public bool EnableSsl { get; set; } = true;
        public bool UseMock { get; set; } = true; // In development: log naar console
    }

    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(
            IOptions<EmailSettings> emailSettings,
            ILogger<EmailSender> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                if (_emailSettings.UseMock)
                {
                    // In development: log naar console
                    await SendMockEmail(email, subject, htmlMessage);
                }
                else
                {
                    // In productie: stuur echte e-mail
                    await SendRealEmail(email, subject, htmlMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fout bij verzenden e-mail naar {email}");
                throw;
            }
        }

        private async Task SendMockEmail(string email, string subject, string htmlMessage)
        {
            // Log naar console voor development
            Console.WriteLine("=".PadRight(60, '='));
            Console.WriteLine($"📧 MOCK E-MAIL VERZONDEN");
            Console.WriteLine($"   Aan: {email}");
            Console.WriteLine($"   Onderwerp: {subject}");
            Console.WriteLine($"   Bericht: {StripHtml(htmlMessage)}");
            Console.WriteLine("=".PadRight(60, '='));

            // Ook loggen via logger
            _logger.LogInformation($"Mock e-mail verzonden naar {email}");
            _logger.LogDebug($"E-mail inhoud: {StripHtml(htmlMessage)}");

            await Task.CompletedTask;
        }

        private async Task SendRealEmail(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrEmpty(_emailSettings.SenderEmail) ||
                string.IsNullOrEmpty(_emailSettings.SenderPassword))
            {
                throw new InvalidOperationException("E-mail settings niet geconfigureerd");
            }

            using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(
                    _emailSettings.SenderEmail,
                    _emailSettings.SenderPassword),
                EnableSsl = _emailSettings.EnableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, "Fitness Club"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation($"Echte e-mail verzonden naar {email}");
        }

        private string StripHtml(string html)
        {
            // Eenvoudige HTML stripper voor logging
            return System.Text.RegularExpressions.Regex.Replace(
                html, "<[^>]*>", string.Empty);
        }
    }
}