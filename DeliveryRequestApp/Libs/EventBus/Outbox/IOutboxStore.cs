using EventBus;

namespace Core.Outbox;

public delegate Task OutboxPublish(IEventBus eventBus, CancellationToken cancellationToken);

public interface IOutboxStore
{
    void Enqueue<T>(T message) where T : class;

    IReadOnlyList<OutboxPublish> Pending { get; }

    void Clear();
}
