using Microsoft.AspNetCore.Mvc;
using SampleApi.Messaging;

namespace SampleApi.Endpoints;

public static class QueuesEndpoints
{
    public static RouteGroupBuilder AddQueuesEndpoints(this RouteGroupBuilder builder)
    {
        var group = builder.MapGroup("queues");

        group.MapGet("/", async (SQSService service, CancellationToken cancellationToken) =>
            await service.ListQueueUrlsAsync(cancellationToken))
        .WithDescription("Returns all queue URLs in the current account");

        group.MapGet("/info", () => Results.Text("""
            Queue URLs and names are case-sensitive.
            After you create a queue, you must wait at least one second after the queue is created to be able to use the queue.
            If you delete a queue, you must wait at least 60 seconds before creating a queue with the same name.
            """,
            System.Net.Mime.MediaTypeNames.Text.Plain));

        group.MapGet("/{queueName}/messages", async (
            SQSService service,
            string queueName,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(queueName))
                return Results.BadRequest();

            var result = await service.ListMessagesAsync(queueName, cancellationToken);
            var dtos = result.ClientResponse.Messages.Select(Mappers.ModelToDto);

            if (result.ClientResponse.Messages.Any())
                _ = await service.DequeueMessagesBatchAsync(result.QueueUrl, result.ClientResponse.Messages, cancellationToken);

            return Results.Ok(dtos);
        })
        .WithDescription("Consumes all messages from the queue, removing them");

        group.MapPost("/{queueName}/messages", async (
            SQSService service,
            string queueName,
            [FromBody] EnqueueMessageDto dto,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(queueName) || dto is null || string.IsNullOrWhiteSpace(dto.Body))
                return Results.BadRequest();

            var result = await service.EnqueueMessageAsync(queueName, dto, cancellationToken);
            return Results.Created(result.QueueUrl, new { result.ClientResponse.MessageId });
        })
        .WithDescription("Enqueues a message in the queue");

        group.MapDelete("/{queueName}/messages/{receiptHandle}", async (
            SQSService service,
            string queueName,
            string receiptHandle,
            CancellationToken cancellationToken) =>
        {
            _ = await service.DequeueMessageAsync(queueName, receiptHandle, cancellationToken);
            return Results.NoContent();
        })
        .WithDescription("Dequeue a message from the queue");

        group.MapDelete("/{queueName}/purge", async (
            SQSService service,
            string queueName,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(queueName))
                return Results.BadRequest();

            _ = await service.PurgeQueueAsync(queueName, cancellationToken);
            return Results.NoContent();
        })
        .WithDescription("Purges the queue");

        group.MapPost("/{queueName}", async (SQSService service, string queueName, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(queueName))
                return Results.BadRequest();

            var response = await service.CreateQueueAsync(queueName, cancellationToken);
            return Results.Created(response.QueueUrl, new { QueueName = queueName });
        })
        .WithDescription("Creates a new standard queue");

        group.MapDelete("/{queueName}", async (
            SQSService service,
            string queueName,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(queueName))
                return Results.BadRequest();

            _ = await service.DeleteQueueAsync(queueName, cancellationToken);
            return Results.NoContent();
        })
        .WithDescription("Deletes an existing queue");

        return builder;
    }
}