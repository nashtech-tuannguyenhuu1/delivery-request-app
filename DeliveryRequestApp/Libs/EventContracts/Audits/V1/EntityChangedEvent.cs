namespace EventContracts.Audits.V1;

public class EntityChangedEvent
{
    public Guid Id { get; set; }

    public string TableName { get; set; } = default!;

    public string Action { get; set; } = default!;

    public DateTime Timestamp { get; set; }

    public string? UserId { get; set; }

    public string? PrimaryKey { get; set; }

    public Guid? TransactionId { get; set; }

    public Guid? CorrelationId { get; set; }

    public ICollection<PropertyDataDto> Properties { get; set; } = new List<PropertyDataDto>();

    public class PropertyDataDto
    {
        public Guid Id { get; set; }

        public Guid AuditId { get; set; }

        public string PropertyName { get; set; } = default!;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }
    }
}
