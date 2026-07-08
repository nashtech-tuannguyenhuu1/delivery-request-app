namespace EventBus;

/// <summary>Invokes the next behaviour in the publish pipeline, or the actual send at the end.</summary>
public delegate Task PublishDelegate();

/// <summary>
/// Cross-cutting behaviour wrapped around <see cref="IEventBus.PublishAsync{T}"/>.
/// Behaviours run before the envelope is serialized, so they may mutate it (e.g. set
/// <see cref="MessageEnvelope{T}.CorrelationId"/> or add headers), and run in
/// registration order (first registered is outermost). Skipping <paramref name="next"/>
/// short-circuits the send.
/// </summary>
public interface IPublishBehaviour<T> where T : class
{
    Task PublishAsync(MessageEnvelope<T> envelope, PublishDelegate next, CancellationToken cancellationToken);
}
