using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace BusinessCentral.Infrastructure.ExternalServices
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailOptions _options;

        public SmtpEmailService(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            using var client = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
            {
                EnableSsl = _options.UseSsl
            };

            if (!string.IsNullOrEmpty(_options.Username))
                client.Credentials = new NetworkCredential(_options.Username, _options.Password);

            var mail = new MailMessage(_options.From, toEmail, subject, htmlBody) { IsBodyHtml = true };
            await client.SendMailAsync(mail);
        }
    }
}