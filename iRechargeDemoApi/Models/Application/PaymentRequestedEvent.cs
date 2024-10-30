namespace iRechargeDemoApi.Models.Application
{
    public class PaymentRequestedEvent
    {
        public string ValidationRef { get; set; }
        public decimal Amount { get; set; }
        public string WalletId { get; set; }
        public DateTime Date { get; set; }
    }
}
