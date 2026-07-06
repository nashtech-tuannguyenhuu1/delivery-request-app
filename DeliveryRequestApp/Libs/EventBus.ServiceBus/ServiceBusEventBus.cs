using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace EventBus.ServiceBus;

/// <summary>
/// Scoped IEventBus: builds the envelope, runs the publish pipeline
/// (IPublishBehaviour&lt;T&gt; chain) which may enrich the envelope, then serializes
/// and sends via the singleton <see cref="ServiceBusSenderRegistry"/>. Serialization
/// happens in the terminal step, so behaviours' mutations are captured. Scoped so
/// behaviours can use request-scoped services (e.g. IHttpContextAccessor).
/// </summary>
internal sealed class ServiceBusEventBus(
    ServiceBusSenderRegistry senders,
    PublishRouteRegistry routes,
    IServiceProvider serviceProvider,
    IEnumerable<IPublishHeaderProvider> headerProviders) : IEventBus
{
    public async Task PublishAsync<T>(T message, PublishOptions? options = null,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(message);

        var destination = options?.Destination ?? routes.Resolve(typeof(T))
            ?? throw new InvalidOperationException(
                $"No destination configured for message type '{typeof(T).Name}'. " +
                "Map it with MapQueueMessage/MapTopicMessage at registration, or set PublishOptions.Destination.");

        // Start from the caller's headers, then let providers (e.g. service name) enrich them.
        var headers = new Dictionary<string, string>(options?.Headers ?? []);
        foreach (var provider in headerProviders)
        {
            provider.Enrich(headers);
        }

        var envelope = new MessageEnvelope<T>
        {
            CorrelationId = options?.CorrelationId,
            Headers = headers,
            MessageType = typeof(T).FullName ?? typeof(T).Name,
            Content = message,
        };

        // Terminal step: serialize the (possibly enriched) envelope and send.
        PublishDelegate pipeline = async () =>
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(envelope, EnvelopeSerializer.Options);
            await senders.SendAsync(destination, body, envelope.MessageId, envelope.CorrelationId,
                envelope.MessageType, envelope.Headers, cancellationToken);
        };

        // Behaviours wrap the terminal, executing in registration order
        // (iterate in reverse to nest the terminal innermost).
        foreach (var behaviour in serviceProvider.GetServices<IPublishBehaviour<T>>().Reverse())
        {
            var next = pipeline;
            var current = behaviour;
            pipeline = () => current.PublishAsync(envelope, next, cancellationToken);
        }

        await pipeline();
    }
}
