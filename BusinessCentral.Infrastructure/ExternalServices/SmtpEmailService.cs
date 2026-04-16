using BusinessCentral.Application.Ports.Outbound;
using BusinessCentral.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MailKit.Security;
using MimeKit;

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
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Tu Nombre", _options.From));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();
            try
            {
                // En .NET 6, StartTls es la opción más segura para el puerto 587
                await client.ConnectAsync(_options.SmtpHost, _options.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_options.Username, _options.Password);
                await client.SendAsync(message);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}