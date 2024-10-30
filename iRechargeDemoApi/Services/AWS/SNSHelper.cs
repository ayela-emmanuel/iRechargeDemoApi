using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using iRechargeDemoApi.Models.Config;
using Microsoft.Extensions.Options;

namespace iRechargeDemoApi.Services.AWS
{
    public class SNSHelper
    {
        private readonly AmazonSimpleNotificationServiceClient _snsClient;

        public SNSHelper(IOptions<AWSConfig> awsOptions)
        {
            var awsConfig = awsOptions.Value;
            var snsConfig = new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = awsConfig.ServiceURL,
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsConfig.Region)
            };

            _snsClient = new AmazonSimpleNotificationServiceClient(
                awsConfig.AccessKey,
                awsConfig.SecretKey,
                snsConfig
            );
        }

        public async Task<string> GetTopicArnAsync(string topicName)
        {
            // var response = await _snsClient.ListTopicsAsync();
            // var topicArn = response.Topics.Find(t => t.TopicArn.EndsWith($":{topicName}"))?.TopicArn;

            // if (string.IsNullOrEmpty(topicArn))
            // {
            //     throw new Exception($"Topic '{topicName}' not found.");
            // }

            return "topicArn";
        }

        public async Task PublishAsync(string topicArn, string message)
        {
            try
            {
                await _snsClient.PublishAsync(new PublishRequest
                {
                    TopicArn = topicArn,
                    Message = message
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to publish message to SNS: {ex.Message}");
                throw;
            }
        }


        public async Task<string> CreateTopicAsync(string topicName)
        {
            var response = await _snsClient.CreateTopicAsync(new CreateTopicRequest
            {
                Name = topicName
            });

            return response.TopicArn;
        }

        public async Task<string> SubscribeAsync(string topicArn, string protocol, string endpoint)
        {
            var response = await _snsClient.SubscribeAsync(new SubscribeRequest
            {
                TopicArn = topicArn,
                Protocol = protocol,
                Endpoint = endpoint
            });

            return response.SubscriptionArn;
        }

        public async Task UnsubscribeAsync(string subscriptionArn)
        {
            await _snsClient.UnsubscribeAsync(new UnsubscribeRequest
            {
                SubscriptionArn = subscriptionArn
            });
        }

        
    }
}
