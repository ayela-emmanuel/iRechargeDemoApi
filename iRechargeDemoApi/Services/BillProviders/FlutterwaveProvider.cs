using iRechargeDemoApi.DataContext;
using iRechargeDemoApi.Models.Application;
using iRechargeDemoApi.Models.Database;

namespace iRechargeDemoApi.Services.BillProviders
{
    public class FlutterwaveProvider : IElectricityBillProvider
    {
        private readonly WalletService walletService;
        private readonly AppDBContext appDBContext;

        public FlutterwaveProvider(WalletService walletService, AppDBContext appDBContext) {
            this.walletService = walletService;
            this.appDBContext = appDBContext;
        }

        
        public async Task<Bill> VerifyBill(BillCreationModel billdata)
        {
            // Simulate Service Delay..
            await Task.Delay(1000);
            var bill = new Bill
            {
                Id = Guid.NewGuid().ToString(),
                Provider = "FlutterWave",
                Amount = billdata.Amount,
                TransactionState = BillTransactionState.Unpaid,
                CreatedDate = DateTime.UtcNow
            };
            appDBContext.AddBill(bill);
            // Simulate bill verification and creation for Flutterwave
            return bill;
        }

        public bool ProcessPayment(Bill bill, string walletID )
        {
            if (walletService.DeductFunds(walletID, bill.Amount))
            {
                // Deduct amount from wallet and mark bill as paid
                bill.TransactionState = BillTransactionState.Paid;
                return true;
            }
            // Insufficient funds
            return false;
        }
    }
}
