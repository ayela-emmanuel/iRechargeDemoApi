using Amazon.SimpleNotificationService.Model;
using iRechargeDemoApi.DataContext;
using iRechargeDemoApi.Models.Application;
using iRechargeDemoApi.Models.Database;
using iRechargeDemoApi.Services;
using iRechargeDemoApi.Services.AWS;
using iRechargeDemoApi.Services.BillProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace iRechargeDemoApi.Controllers
{
    namespace iRechargeDemoApi.Controllers
    {
        [Route("api/")]
        [ApiController]
        public class DemoController : ControllerBase
        {
            private readonly ILogger<DemoController> _logger;
            private readonly WalletService _walletService;
            private readonly FlutterwaveProvider _flutterBillProvider;
            private readonly BuyPowerProvider _buyPowerProvider;
            private readonly AppDBContext _appDBContext;
            private readonly SNSHelper _snsHelper;
            private readonly SQSHelper _sqsHelper;

            public DemoController(
                ILogger<DemoController> logger,
                WalletService walletService,
                FlutterwaveProvider flutterBillProvider,
                BuyPowerProvider buyPowerProvider,
                AppDBContext appDBContext,
                SNSHelper snsHelper,
                SQSHelper sqsHelper)
            {
                _logger = logger;
                _walletService = walletService;
                _flutterBillProvider = flutterBillProvider;
                _buyPowerProvider = buyPowerProvider;
                _appDBContext = appDBContext;
                _snsHelper = snsHelper;
                _sqsHelper = sqsHelper;
            }

            [HttpPost("electricity/verify")]
            public async Task<IActionResult> InitBill([FromBody] BillCreationModel billModel)
            {
                if (billModel.Amount < 100 || billModel.Amount > 1000000)
                {
                    return BadRequest("Amount must be between 100 and 1,000,000.");
                }

                Bill bill;
                try
                {
                    switch (billModel.ProviderName.ToLower())
                    {
                        case "buypower":
                            bill = await _buyPowerProvider.VerifyBill(billModel);
                            break;
                        case "flutter":
                            bill = await _flutterBillProvider.VerifyBill(billModel);
                            break;
                        default:
                            return BadRequest("Invalid vendor.");
                    }

                    if (bill == null)
                    {
                        return BadRequest("Failed to verify bill.");
                    }

                    // Add the bill to the static DB
                    _appDBContext.AddBill(bill);

                    // Publish event to SNS
                    var billCreatedEvent = new BillCreatedEvent
                    {
                        BillId = bill.Id,
                        Provider = bill.Provider,
                        Amount = bill.Amount,
                        CreatedDate = bill.CreatedDate
                    };

                    var message = JsonConvert.SerializeObject(billCreatedEvent);

                    // Use GetTopicArnAsync to get the topic ARN by name
                    var topicArn = await _snsHelper.GetTopicArnAsync("sms");
                    await _snsHelper.PublishAsync(topicArn, message);

                    return Ok(new ApiResponseModel<Bill>
                    {
                        Result = true,
                        Message = "Bill created successfully.",
                        Data = bill
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during bill verification.");
                    return StatusCode(500, "An error occurred while verifying the bill.");
                }
            }

            [HttpPost("vend/{validationRef:guid}/pay")]
            public async Task<IActionResult> Vend(Guid validationRef)
            {
                try
                {
                    var bill = _appDBContext.Bills.FirstOrDefault(b => b.Id == validationRef.ToString());

                    if (bill == null)
                    {
                        return NotFound("Bill not found.");
                    }

                    if (bill.TransactionState != BillTransactionState.Unpaid)
                    {
                        return BadRequest("Bill is already paid or reversed.");
                    }

                    var wallet = _appDBContext.Wallets.FirstOrDefault();
                    if (wallet == null)
                    {
                        return BadRequest("Wallet not found.");
                    }

                    if (wallet.Balance < bill.Amount)
                    {
                        return BadRequest("Insufficient wallet balance.");
                    }

                    // Deduct amount from wallet and update bill status
                    wallet.Balance -= bill.Amount;
                    _appDBContext.UpdateWallet(wallet);

                    bill.TransactionState = BillTransactionState.Paid;
                    _appDBContext.UpdateBill(bill);

                    // Prepare payment event
                    var paymentEvent = new PaymentRequestedEvent
                    {
                        ValidationRef = validationRef.ToString(),
                        Amount = bill.Amount,
                        WalletId = wallet.Id,
                        Date = DateTime.UtcNow
                    };

                    var message = JsonConvert.SerializeObject(paymentEvent);

                    // Use GetTopicArn to get the topic ARN by name
                    var topicArn = await _snsHelper.GetTopicArnAsync("sms");

                    // Publish to SNS
                    await _snsHelper.PublishAsync(topicArn, message);

                    return Accepted(new ApiResponseModel<string>
                    {
                        Result = true,
                        Message = "Payment processing initiated.",
                        Data = validationRef.ToString()
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during payment processing.");

                    // Handle rollback if necessary
                    var bill = _appDBContext.Bills.FirstOrDefault(b => b.Id == validationRef.ToString());
                    var wallet = _appDBContext.Wallets.FirstOrDefault();

                    if (bill != null && wallet != null)
                    {
                        // Reverse the transaction
                        wallet.Balance += bill.Amount;
                        _appDBContext.UpdateWallet(wallet);

                        bill.TransactionState = BillTransactionState.Unpaid;
                        _appDBContext.UpdateBill(bill);
                    }

                    return StatusCode(500, "An error occurred while processing the payment.");
                }
            }

            [HttpGet("wallets")]
            public IActionResult GetWallets()
            {
                var wallets = _appDBContext.Wallets;
                return Ok(new ApiResponseModel<List<Wallet>>
                {
                    Result = true,
                    Message = "Wallets retrieved successfully.",
                    Data = wallets
                });
            }

            [HttpPost("wallets/{id}/add-funds")]
            public async Task<IActionResult> AddFundsToWallet(Guid id, [FromBody] AddFundsModel model)
            {
                if (model == null || model.Amount <= 0)
                {
                    return BadRequest("Invalid amount.");
                }

                var wallet = _appDBContext.Wallets.FirstOrDefault(w => w.Id == id.ToString());
                if (wallet == null)
                {
                    return NotFound("Wallet not found.");
                }

                wallet.Balance += model.Amount;
                _appDBContext.UpdateWallet(wallet);

                // Publish event to SNS
                var fundsAddedEvent = new FundsAddedEvent
                {
                    WalletId = wallet.Id,
                    Amount = model.Amount,
                    Date = DateTime.UtcNow
                };

                var message = JsonConvert.SerializeObject(fundsAddedEvent);
                var topicArn = await _snsHelper.GetTopicArnAsync("sms");
                await _snsHelper.PublishAsync(topicArn, message);

                return Ok(new ApiResponseModel<Wallet>
                {
                    Result = true,
                    Message = "Funds added successfully.",
                    Data = wallet
                });
            }
        }
    }
}
