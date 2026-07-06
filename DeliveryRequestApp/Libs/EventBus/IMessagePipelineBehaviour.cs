namespace EventBus;

/// <summary>Invokes the next behaviour in the pipeline, or the handler itself at the end.</summary>
public delegate Task MessageHandlerDelegate();

/// <summary>
/// Cross-cutting behaviour wrapped around an <see cref="IMessageHandler{T}"/> invocation.
/// Behaviours run in registration order (first registered is outermost); each decides
/// whether/when to call <paramref name="next"/> — enabling logging, validation, timing,
/// exception shaping, etc.
/// </summary>
public interface IMessagePipelineBehaviour<T> where T : class
{
    Task HandleAsync(MessageEnvelope<T> envelope, MessageHandlerDelegate next, CancellationToken cancellationToken);
}
