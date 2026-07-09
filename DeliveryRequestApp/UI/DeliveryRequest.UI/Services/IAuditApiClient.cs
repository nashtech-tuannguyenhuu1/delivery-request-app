using DeliveryRequest.UI.Models.Audits;
using DeliveryRequest.UI.Models.DeliveryRequests;

namespace DeliveryRequest.UI.Services;

public interface IAuditApiClient
{
    Task<ApiResultDto<List<AuditRecordDto>>> GetAuditsAsync(string primaryKey, string? tableName = null, CancellationToken cancellationToken = default);
}
