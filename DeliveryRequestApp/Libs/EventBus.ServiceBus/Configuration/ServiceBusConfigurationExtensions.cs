namespace EventBus.ServiceBus.Configuration;

/// <summary>
/// Validation for a bound ServiceBusConfiguration. Runs automatically inside
/// AddServiceBusEventBus so misconfiguration fails at startup, not at first publish.
/// Collects every problem and throws once with the full list.
/// </summary>
public static class ServiceBusConfigurationExtensions
{
    public static void Validate(this ServiceBusConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var errors = new List<string>();

        if (configuration.AuthenticationDto is null ||
            string.IsNullOrWhiteSpace(configuration.AuthenticationDto.ConnectionString))
        {
            errors.Add("AuthenticationDto.ConnectionString is missing.");
        }

        foreach (var (key, queue) in configuration.Queues)
        {
            if (queue is null || string.IsNullOrWhiteSpace(queue.QueueName))
            {
                errors.Add($"Queues['{key}'].QueueName is missing.");
            }
        }

        foreach (var (key, topic) in configuration.Topics)
        {
            if (topic is null || string.IsNullOrWhiteSpace(topic.TopicName))
            {
                errors.Add($"Topics['{key}'].TopicName is missing.");
            }
        }

        if (configuration.MaxConcurrentCalls < 1)
        {
            errors.Add($"MaxConcurrentCalls must be at least 1 (was {configuration.MaxConcurrentCalls}).");
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                "Invalid ServiceBusConfiguration:" + Environment.NewLine +
                string.Join(Environment.NewLine, errors.Select(e => $"  - {e}")));
        }
    }
}
