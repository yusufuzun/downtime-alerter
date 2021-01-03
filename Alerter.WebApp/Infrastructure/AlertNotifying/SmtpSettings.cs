namespace Alerter.WebApp.Infrastructure.AlertNotifying
{
    public class SmtpSettings
    {
        public string MailSender { get; set; }

        public string SmtpServer { get; set; }

        public int Port { get; set; }

        public string MailSenderName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
