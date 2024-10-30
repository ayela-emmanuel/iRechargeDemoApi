namespace iRechargeDemoApi.Models.Application
{
    public class BillCreatedEvent
    {
        public string BillId { get; set; }
        public string Provider { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
