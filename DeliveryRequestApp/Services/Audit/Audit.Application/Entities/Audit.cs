using Core.Domain;

namespace Audit.Application.Entities;

public class Audit : IEntity<int>
{
    public int Id { get; set; }

    public string TableName { get; set; } = default!;

    public string Action { get; set; } = default!;

    public DateTime Timestamp { get; set; }

    public string? UserId { get; set; }

    public string? PrimaryKey { get; set; }

    public Guid? TransactionId { get; set; }

    public Guid? CorrelationId { get; set; }

    public virtual ICollection<AuditData> AuditProperties { get; set; } = [];
}
