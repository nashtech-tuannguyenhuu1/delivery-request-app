namespace DeliveryRequest.UI.Models.Audits;

/// <summary>Mirrors Audit.Application.Responses.AuditRecordDto returned by the Audit API.</summary>
public record AuditRecordDto
{
    public int Id { get; init; }

    public string TableName { get; init; } = default!;

    public string Action { get; init; } = default!;

    public DateTime Timestamp { get; init; }

    public string? UserId { get; init; }

    public string? PrimaryKey { get; init; }

    public List<AuditPropertyDto> Properties { get; init; } = [];
}

public record AuditPropertyDto
{
    public string PropertyName { get; init; } = default!;

    public string? OldValue { get; init; }

    public string? NewValue { get; init; }
}
