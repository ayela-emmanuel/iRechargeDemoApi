using iRechargeDemoApi.Models.Config;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace iRechargeDemoApi.Services.NotificationProviders
{
    public class MailService
    {
        private readonly IOptions<MailConfig> _mailConfig;

        public MailService(IOptions<MailConfig> mailConfig)
        {
            _mailConfig = mailConfig ?? throw new ArgumentNullException(nameof(mailConfig));
        }

        public void SendMail(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to)) throw new ArgumentException("Recipient email address is required.", nameof(to));
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("Email subject is required.", nameof(subject));
            if (string.IsNullOrWhiteSpace(body)) throw new ArgumentException("Email body is required.", nameof(body));

            // Log the email configuration (mocking sending process)
            Console.WriteLine($"Preparing to send email using SMTP Server: {_mailConfig.Value.SmtpServer}");
            Console.WriteLine($"From: {_mailConfig.Value.SenderEmail}");
            Console.WriteLine($"To: {to}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {body}");

            // real implementation -------

             /*var smtpClient = new SmtpClient(_mailConfig.Value.SmtpServer)
             {
                 Port = _mailConfig.Value.Port,
                 Credentials = new NetworkCredential(_mailConfig.Value.SenderName, _mailConfig.Value.Password),
             };
            
             var mailMessage = new MailMessage
             {
                 From = new MailAddress(_mailConfig.Value.SenderEmail),
                 Subject = subject,
                 Body = body,
             };
             mailMessage.To.Add(to);
            
             smtpClient.Send(mailMessage);*/

            Console.WriteLine("Email sent successfully (mocked).");
        }
    }
}
