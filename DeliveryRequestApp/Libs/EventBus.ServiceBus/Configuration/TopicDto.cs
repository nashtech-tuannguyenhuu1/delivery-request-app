namespace EventBus.ServiceBus.Configuration;

public sealed class TopicDto
{
    public string TopicName { get; set; } = string.Empty;

    /// <summary>Only required on the consumer side.</summary>
    public string? SubscriptionName { get; set; }
}
