namespace EventBus;

/// <summary>
/// Implement to consume messages of type <typeparamref name="T"/>.
/// Receives the full envelope so MessageId, CorrelationId and Headers are available.
/// </summary>
public interface IMessageHandler<T> where T : class
{
    Task HandleAsync(MessageEnvelope<T> envelope, CancellationToken cancellationToken);
}
