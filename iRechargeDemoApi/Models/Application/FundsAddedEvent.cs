namespace iRechargeDemoApi.Models.Application
{
    public class FundsAddedEvent
    {
        public string WalletId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
