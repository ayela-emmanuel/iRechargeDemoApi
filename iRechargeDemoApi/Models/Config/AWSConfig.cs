namespace iRechargeDemoApi.Models.Config
{
    public class AWSConfig
    {
        public required string ServiceURL { get; set; }
        public required string AccessKey { get; set; }
        public required string SecretKey { get; set; }
        public required string Region { get; set; }
    }
}
