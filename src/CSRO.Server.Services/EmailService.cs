using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Services
{
    public interface IEmailService
    {
        Task Send(string from, string to, string subject, string text, bool isHtml);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;
        public EmailService(IConfiguration configuration)
        {
            _emailConfig = configuration.GetSection(nameof(EmailConfig)).Get<EmailConfig>();
        }

        public async Task Send(string from, string to, string subject, string text, bool isHtml)
        {
            try
            {
                // create message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(from));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                if (isHtml)
                    email.Body = new TextPart(TextFormat.Html) { Text = text };
                else
                    email.Body = new TextPart(TextFormat.Plain) { Text = text };

                // send email
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailConfig.SmtpHost, _emailConfig.SmtpPort, SecureSocketOptions.StartTls);
                if (_emailConfig.HasPassword)
                    await smtp.AuthenticateAsync(_emailConfig.SmtpUser, _emailConfig.SmtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (System.Exception ex)
            {
                throw;   
            }
        }
    }
}