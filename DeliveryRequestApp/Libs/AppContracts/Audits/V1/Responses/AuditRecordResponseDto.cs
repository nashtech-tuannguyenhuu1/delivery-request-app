namespace AppContracts.Audits.V1.Responses;

public record AuditRecordResponseDto
{
    public int Id { get; init; }

    public string TableName { get; init; } = default!;

    public string Action { get; init; } = default!;

    public DateTime Timestamp { get; init; }

    public string? UserId { get; init; }

    public string? PrimaryKey { get; init; }

    public List<AuditPropertyResponseDto> Properties { get; init; } = [];
}

public record AuditPropertyResponseDto
{
    public string PropertyName { get; init; } = default!;

    public string? OldValue { get; init; }

    public string? NewValue { get; init; }
}
