using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using iRechargeDemoApi.Models.Config;
using Microsoft.Extensions.Options;

namespace iRechargeDemoApi.Services.AWS
{
    public class SQSHelper
    {
        private readonly AmazonSQSClient _sqsClient;

        public SQSHelper(IOptions<AWSConfig> awsOptions)
        {
            var awsConfig = awsOptions.Value;
            var sqsConfig = new AmazonSQSConfig
            {
                ServiceURL = awsConfig.ServiceURL,
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsConfig.Region)
            };

            _sqsClient = new AmazonSQSClient(
                awsConfig.AccessKey,
                awsConfig.SecretKey,
                sqsConfig
            );
        }

        public async Task<string> CreateQueueAsync(string queueName)
        {
            var response = await _sqsClient.CreateQueueAsync(new CreateQueueRequest
            {
                QueueName = queueName
            });

            return response.QueueUrl;
        }

        public async Task SendMessageAsync(string queueUrl, string message)
        {
            try
            {
                await _sqsClient.SendMessageAsync(new SendMessageRequest
                {
                    QueueUrl = queueUrl,
                    MessageBody = message
                });
            }
            catch (Exception ex)
            {
                // Log and rethrow or handle the exception as needed
                Console.WriteLine($"Failed to send message to SQS: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Message>> ReceiveMessagesAsync(string queueUrl, int maxMessages = 1)
        {
            var response = await _sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = maxMessages
            });

            return response.Messages;
        }

        public async Task DeleteMessageAsync(string queueUrl, string receiptHandle)
        {
            await _sqsClient.DeleteMessageAsync(new DeleteMessageRequest
            {
                QueueUrl = queueUrl,
                ReceiptHandle = receiptHandle
            });
        }
    }
}
