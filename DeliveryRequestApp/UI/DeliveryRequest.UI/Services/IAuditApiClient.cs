using AppContracts.Audits.V1.Responses;
using DeliveryRequest.UI.Models.DeliveryRequests;

namespace DeliveryRequest.UI.Services;

public interface IAuditApiClient
{
    Task<ApiResultDto<List<AuditRecordResponseDto>>> GetAuditsAsync(string primaryKey, string? tableName = null, CancellationToken cancellationToken = default);
}
