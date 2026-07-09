using AppContracts.Reports.V1.Responses;
using DeliveryRequest.UI.Models.DeliveryRequests;

namespace DeliveryRequest.UI.Services;

public interface IReportApiClient
{
    Task<ApiResultDto<ReportTotalResponseDto>> GetReportByDateRangeAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
}
