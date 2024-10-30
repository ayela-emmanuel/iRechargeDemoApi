

using iRechargeDemoApi.Models.Database;

namespace iRechargeDemoApi.DataContext
{
    public class AppDBContext 
    {

        private static List<Bill> BillsData = new List<Bill>
        {
        };
        private static List<Wallet> WalletsData = new List<Wallet>
        {
            new Wallet {
                Id = Guid.NewGuid().ToString(),
                Balance = 1000m,
                CreatedDate = DateTime.UtcNow
            }
        };
        
        public List<Bill> Bills => BillsData;
        public List<Wallet> Wallets => WalletsData;

        // Add a new bill (simulate DB insert)
        public void AddBill(Bill bill)
        {
            BillsData.Add(bill);
        }

        // Add a new wallet (simulate DB insert)
        public void AddWallet(Wallet wallet)
        {
            WalletsData.Add(wallet);
        }

        // Update a bill (simulate DB update)
        public void UpdateBill(Bill bill)
        {
            var existingBill = BillsData.FirstOrDefault(b => b.Id == bill.Id);
            if (existingBill != null)
            {
                existingBill.Amount = bill.Amount;
                existingBill.TransactionState = bill.TransactionState;
                existingBill.CreatedDate = bill.CreatedDate;
            }
        }

        // Update a wallet (simulate DB update)
        public void UpdateWallet(Wallet wallet)
        {
            var existingWallet = WalletsData.FirstOrDefault(w => w.Id == wallet.Id);
            if (existingWallet != null)
            {
                existingWallet.Balance = wallet.Balance;
                existingWallet.CreatedDate = wallet.CreatedDate;
            }
        }


    }
}
