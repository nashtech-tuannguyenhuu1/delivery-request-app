namespace DeliveryRequest.UI.Models.Audits;

public class AuditHistoryViewModel
{
    public IReadOnlyList<AuditRecordDto> Records { get; set; } = Array.Empty<AuditRecordDto>();

    public string? ErrorMessage { get; set; }
}
