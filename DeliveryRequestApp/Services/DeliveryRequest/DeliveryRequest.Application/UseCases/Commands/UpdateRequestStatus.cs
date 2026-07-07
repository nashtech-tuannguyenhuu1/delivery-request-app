using AppContracts.DeliveryRequests.V1;
using Core.Caching;
using Core.Data;
using Core.Domain;
using DeliveryRequest.Application.Entities;
using Mediator.Abstractions;

namespace DeliveryRequest.Application.UseCases.Commands;

public class UpdateRequestStatus
{
    public record Command(Guid Id, RequestStatus Status, string? Reason) : ICommand<bool>;

    internal class Handler(IUnitOfWork unitOfWork, ICurrentUser currentUser, ICacheManager cacheManager) : IRequestHandler<Command, ResultModel<bool>>
    {
        public async Task<ResultModel<bool>> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.Repository<Request>()
                .FirstOrDefaultAsync(
                    predicate: x => x.Id == request.Id && x.CreatedById == currentUser.UserId,
                    tracking: true,
                    ct: cancellationToken);

            if (entity is null)
            {
                return ResultModel<bool>.Create(false, isError: true, errorMessage: "Request not found.");
            }

            if (request.Status == RequestStatus.Returned)
            {
                entity.ReturnedReason = request.Reason;
            }

            if (request.Status == RequestStatus.Delivered)
            {
                entity.DeliveredDate = DateTimeOffset.Now;
            }

            entity.StatusId = (int)request.Status;

            unitOfWork.Repository<Request>().Update(entity);

            await cacheManager.RemoveAsync(CacheKeys.CacheKeys.GetRequestKey(entity.Id));

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResultModel<bool>(true);
        }
    }
}
