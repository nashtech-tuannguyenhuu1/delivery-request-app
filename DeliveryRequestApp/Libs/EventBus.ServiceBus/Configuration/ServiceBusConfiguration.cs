namespace EventBus.ServiceBus.Configuration;

/// <summary>
/// Bound from the "ServiceBusConfiguration" section of appsettings.json in each service.
/// Topics and Queues are keyed by a logical name (e.g. "UserEvents", "Orders") that the
/// registration API refers to, so entity names live in configuration only.
/// </summary>
public sealed class ServiceBusConfiguration
{
    public AuthenticationDto AuthenticationDto { get; set; } = new();

    public Dictionary<string, TopicDto> Topics { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, QueueDto> Queues { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Max messages each processor handles concurrently.</summary>
    public int MaxConcurrentCalls { get; set; } = 4;
}
