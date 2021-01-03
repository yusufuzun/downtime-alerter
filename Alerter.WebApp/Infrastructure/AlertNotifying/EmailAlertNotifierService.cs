using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;

namespace Alerter.WebApp.Infrastructure.AlertNotifying
{
    public class EmailAlertNotifierService : IAlertNotifierService
    {
        private readonly SmtpSettings smtpSettings;
        private readonly ISmtpClient smtpClient;
        private readonly ILogger<EmailAlertNotifierService> logger;

        public EmailAlertNotifierService(
            IOptions<SmtpSettings> smtpSettings,
            ISmtpClient smtpClient,
            ILogger<EmailAlertNotifierService> logger)
        {
            this.smtpSettings = smtpSettings.Value;
            this.smtpClient = smtpClient;
            this.logger = logger;
        }

        public async Task SendNotificationAsync(Dictionary<string, string> keyValuePairs)
        {
            if (!keyValuePairs.ContainsKey("email") || !keyValuePairs.ContainsKey("subject") || !keyValuePairs.ContainsKey("message"))
            {
                throw new ArgumentException("not enough arguments");
            }

            await SendEmailAsync(keyValuePairs["email"], keyValuePairs["subject"], keyValuePairs["message"]);
        }

        private async Task SendEmailAsync(string email, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(smtpSettings.MailSenderName, smtpSettings.MailSender));
                message.To.Add(new MailboxAddress(email));
                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = body
                };

                using (smtpClient)
                {
                    await smtpClient.ConnectAsync(smtpSettings.SmtpServer, smtpSettings.Port, SecureSocketOptions.StartTls);

                    await smtpClient.AuthenticateAsync(smtpSettings.Username, smtpSettings.Password);

                    await smtpClient.SendAsync(message);

                    await smtpClient.DisconnectAsync(true);
                }
            }
            catch (Exception e)
            {
                logger.LogError(new EventId(2000, "EmailSendError"), e, "");
            }
        }
    }
}
