namespace iRechargeDemoApi.Models.Database
{
    public class Bill
    {
        public required string Id { get; set; }
        public required string Provider { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public BillTransactionState TransactionState { get; set; }
    }
    public enum BillTransactionState
    {
        Paid,
        Unpaid,
        Reversed
    }
}
