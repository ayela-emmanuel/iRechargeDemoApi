using iRechargeDemoApi.Models.Config;
using Microsoft.Extensions.Options;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace iRechargeDemoApi.Services.NotificationProviders
{
    public class SmsService
    {
        private readonly IOptions<TwilioConfig> _twilioConfig;

        public SmsService(IOptions<TwilioConfig> twilioConfig)
        {
            _twilioConfig = twilioConfig ?? throw new ArgumentNullException(nameof(twilioConfig));
        }

        public void SendSms(string to, string message)
        {
            if (string.IsNullOrWhiteSpace(to)) throw new ArgumentException("Recipient phone number is required.", nameof(to));
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Message content is required.", nameof(message));

            // Log the SMS configuration (mocking sending process)
            Console.WriteLine($"Preparing to send SMS using Twilio Account SID: {_twilioConfig.Value.AccountSid}");
            Console.WriteLine($"To: {to}");
            Console.WriteLine($"Message: {message}");

            // Uncomment below for real implementation
             /*var client = new TwilioRestClient(_twilioConfig.Value.AccountSid, _twilioConfig.Value.AuthToken);
             var messageResponse = MessageResource.Create(
                 to: new PhoneNumber(to),
                 from: new PhoneNumber(_twilioConfig.Value.PhoneNumber),
                 body: message
             );*/

            Console.WriteLine("SMS sent successfully (mocked).");
        }
    }

}
