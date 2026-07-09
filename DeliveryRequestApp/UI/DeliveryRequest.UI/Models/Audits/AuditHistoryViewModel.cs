using AppContracts.Audits.V1.Responses;

namespace DeliveryRequest.UI.Models.Audits;

public class AuditHistoryViewModel
{
    public IReadOnlyList<AuditRecordResponseDto> Records { get; set; } = Array.Empty<AuditRecordResponseDto>();

    public string? ErrorMessage { get; set; }
}
