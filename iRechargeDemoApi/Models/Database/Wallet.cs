namespace iRechargeDemoApi.Models.Database
{
    public class Wallet
    {
        public required string Id { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
