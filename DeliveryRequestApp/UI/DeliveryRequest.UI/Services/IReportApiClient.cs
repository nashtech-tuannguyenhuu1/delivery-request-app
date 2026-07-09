using DeliveryRequest.UI.Models.DeliveryRequests;
using DeliveryRequest.UI.Models.Reports;

namespace DeliveryRequest.UI.Services;

public interface IReportApiClient
{
    Task<ApiResultDto<ReportTotalDto>> GetReportByDateRangeAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
}
