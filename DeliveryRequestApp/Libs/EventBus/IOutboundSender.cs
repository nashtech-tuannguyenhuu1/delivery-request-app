namespace EventBus;

/// <summary>
/// Sends an already-serialized message straight to the broker, bypassing the publish
/// pipeline. For infrastructure that holds ready-to-send bytes and must not re-enter
/// the behaviours — e.g. an outbox relay.
/// </summary>
public interface IOutboundSender
{
    Task SendAsync(string destination, byte[] body, string messageId, string? correlationId,
        string messageType, IReadOnlyDictionary<string, string> headers, CancellationToken cancellationToken);
}
