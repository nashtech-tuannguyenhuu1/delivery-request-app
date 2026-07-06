using EventContracts.Audits.V1;

namespace Infrastructure.Audits;

public interface IAuditEventStore
{
    void Add(EntityChangedEvent audit);

    IReadOnlyList<EntityChangedEvent> GetAll();

    void Clear();
}
