namespace iRechargeDemoApi.Models.Config
{
    public class TwilioConfig
    {
        public required string AccountSid { get; set; }
        public required string AuthToken { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
