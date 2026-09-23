using Amazon.SQS.Model;

namespace SampleApi.Messaging;

public sealed record EnqueueMessageDto(string Body, IReadOnlyDictionary<string, string> Attributes);

public sealed record EnqueueMessageResult(string QueueUrl, SendMessageResponse ClientResponse);

public sealed record ListMessagesResult(string QueueUrl, ReceiveMessageResponse ClientResponse);

public sealed record QueueMessageDto(string? Id, string? Body, string? ReceiptHandle)
{
    public IReadOnlyList<QueueMessageAttributeDto> Attributes { get; set; } = [];
}

public sealed record QueueMessageAttributeDto(string Key, string StringValue);