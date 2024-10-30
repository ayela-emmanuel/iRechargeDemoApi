using iRechargeDemoApi.DataContext;
using iRechargeDemoApi.Models.Database;

namespace iRechargeDemoApi.Services
{
    public class WalletService
    {
        private readonly AppDBContext _dbContext;

        public WalletService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Retrieve wallet details by ID
        public Wallet GetWalletById(string walletId)
        {
            var wallet = _dbContext.Wallets.FirstOrDefault(w => w.Id == walletId);
            if (wallet == null)
            {
                throw new Exception("Wallet not found.");
            }
            return wallet;
        }

        // Add funds to a users wallet
        public void AddFunds(string walletId, decimal amount)
        {
            var wallet = GetWalletById(walletId);
            wallet.Balance += amount;
        }

        // Deduct funds from a users wallet
        public bool DeductFunds(string walletId, decimal amount)
        {
            var wallet = GetWalletById(walletId);
            if (wallet.Balance >= amount)
            {
                wallet.Balance -= amount;
                return true;
            }
            return false; // Insufficient funds
        }

        // Check if a wallet has sufficient funds
        public bool HasSufficientFunds(string walletId, decimal amount)
        {
            var wallet = GetWalletById(walletId);
            return wallet.Balance >= amount;
        }
    }

}
