using Core.Domain;

namespace Audit.Application.Entities;

public class AuditData : IEntity<int>
{
    public int Id { get; set; }

    public int AuditId { get; set; }

    public string PropertyName { get; set; } = default!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }
}
