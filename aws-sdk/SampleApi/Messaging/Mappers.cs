using Amazon.SQS.Model;

namespace SampleApi.Messaging;

public static class Mappers
{
    public static QueueMessageDto ModelToDto(this Message model)
    {
        return new QueueMessageDto(model.MessageId, model.Body, model.ReceiptHandle)
        {
            Attributes = model.MessageAttributes
                .Select(attribute =>
                    new QueueMessageAttributeDto(attribute.Key, attribute.Value.StringValue))
                .ToArray()
        };
    }

    public static Dictionary<string, MessageAttributeValue> MapToModel(this IReadOnlyDictionary<string, string> attributes)
    {
        return attributes.ToDictionary(
            keyValue => keyValue.Key,
            keyValue => new MessageAttributeValue
            {
                DataType = "String",
                StringValue = keyValue.Value
            });
    }
}