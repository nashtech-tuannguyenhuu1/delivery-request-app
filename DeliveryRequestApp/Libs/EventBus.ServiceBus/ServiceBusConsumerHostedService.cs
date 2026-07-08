using Azure.Messaging.ServiceBus;
using EventBus.ServiceBus.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EventBus.ServiceBus;

/// <summary>
/// Runs one ServiceBusProcessor per queue / topic-subscription that has registered handlers,
/// and dispatches each received message to its typed IMessageHandler&lt;T&gt; in a fresh DI scope.
/// </summary>
internal sealed class ServiceBusConsumerHostedService(
    ServiceBusClient client,
    IEnumerable<HandlerRegistration> registrations,
    ServiceBusConfiguration configuration,
    IServiceScopeFactory scopeFactory,
    ILogger<ServiceBusConsumerHostedService> logger) : IHostedService, IAsyncDisposable
{
    private readonly List<ServiceBusProcessor> _processors = [];

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var group in registrations.GroupBy(r => r.Entity))
        {
            var entity = group.Key;
            var invokers = group.ToDictionary(r => r.Invoker.MessageTypeName, r => r.Invoker);

            var options = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = configuration.MaxConcurrentCalls,
            };

            var processor = entity.Subscription is null
                ? client.CreateProcessor(entity.Name, options)
                : client.CreateProcessor(entity.Name, entity.Subscription, options);

            processor.ProcessMessageAsync += args => ProcessMessageAsync(args, entity, invokers);
            processor.ProcessErrorAsync += args =>
            {
                logger.LogError(args.Exception,
                    "Service Bus error on {Entity} (source: {ErrorSource})", entity, args.ErrorSource);
                return Task.CompletedTask;
            };

            _processors.Add(processor);
            await processor.StartProcessingAsync(cancellationToken);
            logger.LogInformation("Started processor for {Entity} handling [{MessageTypes}]",
                entity, string.Join(", ", invokers.Keys));
        }
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args, EntityPath entity,
        IReadOnlyDictionary<string, IMessageHandlerInvoker> invokers)
    {
        var message = args.Message;

        if (!message.ApplicationProperties.TryGetValue(ServiceBusSenderRegistry.MessageTypeProperty, out var typeValue)
            || typeValue is not string messageType)
        {
            logger.LogWarning("Dead-lettering message {MessageId} on {Entity}: missing MessageType property",
                message.MessageId, entity);
            await args.DeadLetterMessageAsync(message, "MissingMessageType",
                "ApplicationProperties did not contain a MessageType string.", args.CancellationToken);
            return;
        }

        if (!invokers.TryGetValue(messageType, out var invoker))
        {
            // Redelivery can never fix an unknown type — dead-letter immediately.
            logger.LogWarning("Dead-lettering message {MessageId} on {Entity}: no handler for type {MessageType}",
                message.MessageId, entity, messageType);
            await args.DeadLetterMessageAsync(message, "UnknownMessageType",
                $"No handler registered for message type '{messageType}'.", args.CancellationToken);
            return;
        }

        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            await invoker.InvokeAsync(message.Body, scope.ServiceProvider, args.CancellationToken);
            await args.CompleteMessageAsync(message, args.CancellationToken);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Dead-lettering message {MessageId} on {Entity}: invalid payload",
                message.MessageId, entity);
            await args.DeadLetterMessageAsync(message, "InvalidPayload", ex.Message, args.CancellationToken);
        }
        catch (Exception ex)
        {
            // Abandon → Service Bus redelivers; after MaxDeliveryCount it dead-letters automatically.
            logger.LogError(ex,
                "Handler failed for message {MessageId} ({MessageType}) on {Entity}, attempt {DeliveryCount} — abandoning",
                message.MessageId, messageType, entity, message.DeliveryCount);
            await args.AbandonMessageAsync(message, cancellationToken: args.CancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var processor in _processors)
        {
            await processor.StopProcessingAsync(cancellationToken);
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var processor in _processors)
        {
            await processor.DisposeAsync();
        }
    }
}
