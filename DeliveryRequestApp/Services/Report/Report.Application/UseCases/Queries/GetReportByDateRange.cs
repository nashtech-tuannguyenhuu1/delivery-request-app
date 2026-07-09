using Core.Data;
using Core.Domain;
using Mediator.Abstractions;
using Report.Application.Entities;
using Report.Application.Responses;

namespace Report.Application.UseCases.Queries;

public class GetReportByDateRange
{
    public record Query(DateOnly From, DateOnly To) : IQuery<ReportTotalDto>;

    internal class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, ResultModel<ReportTotalDto>>
    {
        public async Task<ResultModel<ReportTotalDto>> HandleAsync(Query request, CancellationToken cancellationToken)
        {
            if (request.From > request.To)
            {
                return ResultModel<ReportTotalDto>.Create(null, isError: true, errorMessage: "'From' date must be on or before 'To' date.");
            }

            var rows = await unitOfWork.Repository<DailyTracking>().FindAsync(
                selector: x => x,
                predicate: x => x.Date >= request.From && x.Date <= request.To,
                ct: cancellationToken);

            var total = new ReportTotalDto
            {
                From = request.From,
                To = request.To,
                NewCount = rows.Sum(x => x.NewCount),
                AssignedCount = rows.Sum(x => x.AssignedCount),
                DeliveredCount = rows.Sum(x => x.DeliveredCount),
                ReturnedCount = rows.Sum(x => x.ReturnedCount),
            };

            return ResultModel<ReportTotalDto>.Create(total);
        }
    }
}
