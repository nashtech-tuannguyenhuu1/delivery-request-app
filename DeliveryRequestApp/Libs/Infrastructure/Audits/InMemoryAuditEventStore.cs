using EventContracts.Audits.V1;

namespace Infrastructure.Audits;

public class InMemoryAuditEventStore : IAuditEventStore
{
    private readonly List<EntityChangedEvent> _audits = new();

    public void Add(EntityChangedEvent audit) => _audits.Add(audit);

    public IReadOnlyList<EntityChangedEvent> GetAll() => _audits.AsReadOnly();

    public void Clear() => _audits.Clear();
}
