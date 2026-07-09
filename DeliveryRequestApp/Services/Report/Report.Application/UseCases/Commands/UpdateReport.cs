using Core.Data;
using Core.Domain;
using Mediator.Abstractions;
using Report.Application.Entities;
using Report.Application.Enums;

namespace Report.Application.UseCases.Commands;

public class UpdateReport
{
    public record Command(Guid RequestId, int Status) : ICommand<bool>;

    internal class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, ResultModel<bool>>
    {
        public async Task<ResultModel<bool>> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            var dateNow = DateOnly.FromDateTime(DateTime.UtcNow);

            var tracking = await unitOfWork.Repository<DailyTracking>()
                .FirstOrDefaultAsync(x => x.Date == dateNow, tracking: true);

            var currentRequest = await unitOfWork.Repository<RequestStatusTracking>()
                .FirstOrDefaultAsync(x => x.RequestId == request.RequestId, tracking: true);
            var isNew = false;
            if (currentRequest == null)
            {
                isNew = true;
                currentRequest = new RequestStatusTracking
                {
                    Id = Guid.NewGuid(),
                    RequestId = request.RequestId,
                    Status = request.Status,
                };
                await unitOfWork.Repository<RequestStatusTracking>().AddAsync(currentRequest);
            }

            if (tracking == null)
            {
                tracking = new DailyTracking
                {
                    Date = dateNow,
                    Id = Guid.NewGuid(),
                };

                await unitOfWork.Repository<DailyTracking>().AddAsync(tracking);
            }

            if (currentRequest.Status == request.Status)
            {
                if (isNew)
                {
                    tracking.NewCount++;
                }
                await unitOfWork.SaveChangesAsync();
                return ResultModel<bool>.Create(true);
            }

            if (request.Status == (int)DeliveryStatus.New)
            {
                tracking.NewCount += 1;
            }

            if (request.Status == (int)DeliveryStatus.Returned)
            {
                tracking.ReturnedCount += 1;
            }

            if (request.Status == (int)DeliveryStatus.Assigned)
            {
                tracking.AssignedCount += 1;
            }

            if (request.Status == (int)DeliveryStatus.Delivered)
            {
                tracking.DeliveredCount += 1;
            }

            await unitOfWork.SaveChangesAsync();

            return ResultModel<bool>.Create(true);
        }
    }
}
