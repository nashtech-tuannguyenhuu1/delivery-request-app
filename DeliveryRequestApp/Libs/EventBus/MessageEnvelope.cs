namespace EventBus;

/// <summary>
/// Wrapper carrying transport metadata alongside the strongly-typed message content.
/// The whole envelope is serialized as the message body.
/// </summary>
public sealed class MessageEnvelope<T> where T : class
{
    public string MessageId { get; init; } = Guid.NewGuid().ToString();

    // Settable so publish behaviours can enrich it before the envelope is serialized.
    public string CorrelationId { get; set; }

    public Dictionary<string, string> Headers { get; init; } = [];

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    public required string MessageType { get; init; }

    public required T Content { get; init; }
}
