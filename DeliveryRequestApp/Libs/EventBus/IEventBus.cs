namespace EventBus;

public interface IEventBus
{
    Task PublishAsync<T>(T message, PublishOptions? options = null,
        CancellationToken cancellationToken = default) where T : class;
}
