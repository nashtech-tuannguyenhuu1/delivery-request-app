using Azure.Messaging.ServiceBus;
using System.Collections.Concurrent;

namespace EventBus.ServiceBus;

/// <summary>
/// Singleton owner of the ServiceBusClient and a per-entity sender cache.
/// Performs the actual send, mapping an <see cref="OutboundMessage"/> to a native
/// ServiceBusMessage. Shared by the (scoped) event bus and the outbox relay.
/// </summary>
internal sealed class ServiceBusSenderRegistry(ServiceBusClient client) : IOutboundSender, IAsyncDisposable
{
    internal const string MessageTypeProperty = "MessageType";

    private readonly ConcurrentDictionary<string, ServiceBusSender> _senders = new();
    private bool _disposed;

    public async Task SendAsync(string destination, byte[] body, string messageId, string? correlationId,
        string messageType, IReadOnlyDictionary<string, string> headers, CancellationToken cancellationToken)
    {
        var sbMessage = new ServiceBusMessage(body)
        {
            MessageId = messageId,
            CorrelationId = correlationId,
            ContentType = "application/json",
        };

        // Metadata is duplicated into ApplicationProperties so subscription rules
        // can filter without parsing the JSON body.
        sbMessage.ApplicationProperties[MessageTypeProperty] = messageType;
        foreach (var (key, value) in headers)
        {
            sbMessage.ApplicationProperties[key] = value;
        }

        var sender = _senders.GetOrAdd(destination, static (name, c) => c.CreateSender(name), client);
        await sender.SendMessageAsync(sbMessage, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        // Guard: this instance is registered under two service types, so disposal
        // can be requested more than once by the container.
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        foreach (var sender in _senders.Values)
        {
            await sender.DisposeAsync();
        }
    }
}
