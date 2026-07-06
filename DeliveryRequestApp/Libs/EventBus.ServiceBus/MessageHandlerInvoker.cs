using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace EventBus.ServiceBus;

/// <summary>
/// Non-generic bridge from a raw message body to a typed IMessageHandler&lt;T&gt;.
/// A closed-generic instance is created once at registration time, so the
/// receive path is a dictionary lookup — no reflection per message.
/// </summary>
internal interface IMessageHandlerInvoker
{
    string MessageTypeName { get; }

    Task InvokeAsync(BinaryData body, IServiceProvider scopedProvider, CancellationToken cancellationToken);
}

internal sealed class MessageHandlerInvoker<TMessage> : IMessageHandlerInvoker where TMessage : class
{
    // Keyed by full type name (namespace + type) so contracts in different
    // namespaces can never collide; must match the publisher's MessageType.
    public string MessageTypeName => typeof(TMessage).FullName ?? typeof(TMessage).Name;

    public async Task InvokeAsync(BinaryData body, IServiceProvider scopedProvider, CancellationToken cancellationToken)
    {
        var envelope = JsonSerializer.Deserialize<MessageEnvelope<TMessage>>(body, EnvelopeSerializer.Options)
            ?? throw new JsonException($"Message body deserialized to null for '{MessageTypeName}'.");

        var handler = scopedProvider.GetRequiredKeyedService<IMessageHandler<TMessage>>(MessageTypeName);

        // Build the pipeline: behaviours wrap the handler, executing in registration order
        // (GetServices returns registration order, so iterate in reverse to nest correctly).
        var behaviours = scopedProvider.GetServices<IMessagePipelineBehaviour<TMessage>>();

        MessageHandlerDelegate pipeline = () => handler.HandleAsync(envelope, cancellationToken);
        foreach (var behaviour in behaviours.Reverse())
        {
            var next = pipeline;
            var current = behaviour;
            pipeline = () => current.HandleAsync(envelope, next, cancellationToken);
        }

        await pipeline();
    }
}
