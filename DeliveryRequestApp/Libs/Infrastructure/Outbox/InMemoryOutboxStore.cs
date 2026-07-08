using Core.Outbox;

namespace Infrastructure.Outbox;

public sealed class OutboxStore : IOutboxStore
{
    private readonly List<OutboxPublish> _pending = [];

    public IReadOnlyList<OutboxPublish> Pending => _pending;

    public void Enqueue<T>(T message) where T : class
    {
        ArgumentNullException.ThrowIfNull(message);
        _pending.Add((eventBus, cancellationToken) => eventBus.PublishAsync(message, cancellationToken: cancellationToken));
    }

    public void Clear() => _pending.Clear();
}
