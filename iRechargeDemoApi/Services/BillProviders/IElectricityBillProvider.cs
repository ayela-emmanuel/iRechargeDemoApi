using iRechargeDemoApi.Models.Application;
using iRechargeDemoApi.Models.Database;

namespace iRechargeDemoApi.Services.BillProviders
{
    public interface IElectricityBillProvider
    {
        public Task<Bill> VerifyBill(BillCreationModel billdata);
        public bool ProcessPayment(Bill bill, string walletID);
    }
}
