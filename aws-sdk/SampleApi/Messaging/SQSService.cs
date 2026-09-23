using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Caching.Memory;

namespace SampleApi.Messaging;

public sealed partial class SQSService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger<SQSService> _logger;
    private readonly IMemoryCache _cache;

    public SQSService(IAmazonSQS sqsClient, IMemoryCache cache, ILogger<SQSService> logger)
    {
        _sqsClient = sqsClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IReadOnlyList<string>> ListQueueUrlsAsync(CancellationToken cancellationToken)
    {
        var response = await _sqsClient.ListQueuesAsync(string.Empty, cancellationToken);
        return response.QueueUrls;
    }

    public async Task<ListMessagesResult> ListMessagesAsync(string queueName, CancellationToken cancellationToken)
    {
        var queueUrl = await GetQueueUrlAsync(queueName, cancellationToken);
        var request = new ReceiveMessageRequest(queueUrl)
        {
            MessageAttributeNames = { "All" },
            WaitTimeSeconds = 2,
            MaxNumberOfMessages = 10
        };
        var response = await _sqsClient.ReceiveMessageAsync(request, cancellationToken);
        return new ListMessagesResult(queueUrl!, response);
    }

    public async Task<EnqueueMessageResult> EnqueueMessageAsync(string queueName, EnqueueMessageDto dto, CancellationToken cancellationToken)
    {
        var queueUrl = await GetQueueUrlAsync(queueName, cancellationToken);
        var request = new SendMessageRequest(queueUrl, dto.Body);

        if (dto.Attributes is { })
            request.MessageAttributes = dto.Attributes.MapToModel();

        var response = await _sqsClient.SendMessageAsync(request, cancellationToken);

        return new EnqueueMessageResult(queueUrl!, response);
    }

    public async Task<DeleteMessageResponse> DequeueMessageAsync(string queueName, string receiptHandle, CancellationToken cancellationToken)
    {
        var queueUrl = await GetQueueUrlAsync(queueName, cancellationToken);
        return await _sqsClient.DeleteMessageAsync(queueUrl, receiptHandle, cancellationToken);
    }

    public async Task<DeleteMessageBatchResponse> DequeueMessagesBatchAsync(string queueUrl, IReadOnlyList<Message> messages, CancellationToken cancellationToken)
    {
        var request = new DeleteMessageBatchRequest
        {
            QueueUrl = queueUrl,
            Entries = messages
                .Select(message => new DeleteMessageBatchRequestEntry(message.MessageId, message.ReceiptHandle))
                .ToList()
        };

        return await _sqsClient.DeleteMessageBatchAsync(request, cancellationToken);
    }

    public async Task<PurgeQueueResponse> PurgeQueueAsync(string queueName, CancellationToken cancellationToken)
    {
        var queueUrl = await GetQueueUrlAsync(queueName, cancellationToken);
        return await _sqsClient.PurgeQueueAsync(queueUrl, cancellationToken);
    }

    public async Task<CreateQueueResponse> CreateQueueAsync(string queueName, CancellationToken cancellationToken)
    {
        var request = new CreateQueueRequest(queueName)
        {
            Attributes = new Dictionary<string, string>
            {
                { QueueAttributeName.MaximumMessageSize, "1024" },
                { QueueAttributeName.MessageRetentionPeriod, "86400" }
            }
        };

        return await _sqsClient.CreateQueueAsync(request, cancellationToken);
    }

    public async Task<DeleteQueueResponse> DeleteQueueAsync(string queueName, CancellationToken cancellationToken)
    {
        var queueUrl = await GetQueueUrlAsync(queueName, cancellationToken);
        return await _sqsClient.DeleteQueueAsync(queueUrl, cancellationToken);
    }

    private async Task<string?> GetQueueUrlAsync(string queueName, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync($"AmazonSQS:queueUrl:{queueName}", async entry =>
        {
            LogCacheMiss(_logger, entry.Key);

            entry
                .SetSlidingExpiration(TimeSpan.FromMinutes(1))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            var response = await _sqsClient.GetQueueUrlAsync(queueName, cancellationToken);
            return response.QueueUrl;
        });
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Cache miss {CacheKey}")]
    static partial void LogCacheMiss(ILogger logger, object cacheKey);
}