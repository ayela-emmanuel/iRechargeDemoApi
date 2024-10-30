using System.ComponentModel.DataAnnotations;

namespace iRechargeDemoApi.Models.Application
{
    public class BillCreationModel
    {
        [Required]
        public Decimal Amount { get; set; }
        public required string Customer { get; set; }
        public required string WalletId { get; set; }
        [AllowedValues("buypower", "flutter")]
        public required string ProviderName { get; set; }
    }

}
