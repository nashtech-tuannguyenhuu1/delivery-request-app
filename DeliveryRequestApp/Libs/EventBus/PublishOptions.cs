namespace EventBus;

public sealed class PublishOptions
{
    public string? CorrelationId { get; set; }

    public Dictionary<string, string> Headers { get; set; } = [];

    /// <summary>
    /// Queue or topic name to publish to, overriding the destination configured for the message type.
    /// </summary>
    public string? Destination { get; set; }
}
