namespace iRechargeDemoApi.Models.Config
{
    public class MailConfig
    {
        public required string SmtpServer { get; set; }
        public int Port { get; set; } = 587;
        public required string SenderEmail { get; set; }
        public required string SenderName { get; set; }
        public required string Password { get; set; }
    }
}
