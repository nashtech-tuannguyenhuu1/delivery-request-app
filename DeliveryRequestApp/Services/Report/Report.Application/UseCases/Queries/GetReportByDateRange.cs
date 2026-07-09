using AppContracts.Reports.V1.Responses;
using Core.Data;
using Core.Domain;
using Mediator.Abstractions;
using Report.Application.Entities;

namespace Report.Application.UseCases.Queries;

public class GetReportByDateRange
{
    public record Query(DateOnly From, DateOnly To) : IQuery<ReportTotalResponseDto>;

    internal class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, ResultModel<ReportTotalResponseDto>>
    {
        public async Task<ResultModel<ReportTotalResponseDto>> HandleAsync(Query request, CancellationToken cancellationToken)
        {
            if (request.From > request.To)
            {
                return ResultModel<ReportTotalResponseDto>.Create(null, isError: true, errorMessage: "'From' date must be on or before 'To' date.");
            }

            var rows = await unitOfWork.Repository<DailyTracking>().FindAsync(
                selector: x => x,
                predicate: x => x.Date >= request.From && x.Date <= request.To,
                ct: cancellationToken);

            var total = new ReportTotalResponseDto
            {
                From = request.From,
                To = request.To,
                NewCount = rows.Sum(x => x.NewCount),
                AssignedCount = rows.Sum(x => x.AssignedCount),
                DeliveredCount = rows.Sum(x => x.DeliveredCount),
                ReturnedCount = rows.Sum(x => x.ReturnedCount),
            };

            return ResultModel<ReportTotalResponseDto>.Create(total);
        }
    }
}
