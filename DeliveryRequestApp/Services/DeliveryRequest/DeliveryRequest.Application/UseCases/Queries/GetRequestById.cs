using AppContracts.DeliveryRequests.V1.Responses;
using Core.Caching;
using Core.Data;
using Core.Domain;
using DeliveryRequest.Application.Entities;
using Mediator.Abstractions;

namespace DeliveryRequest.Application.UseCases.Queries;

public class GetRequestById
{
    public record Query(Guid Id) : IQuery<RequestResponseDto>;

    internal class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ICacheManager cacheManager
    ) : IRequestHandler<Query, ResultModel<RequestResponseDto>>
    {
        public async Task<ResultModel<RequestResponseDto>> HandleAsync(Query request, CancellationToken cancellationToken)
        {
            var result = await cacheManager.GetOrCreateAsync(CacheKeys.CacheKeys.GetRequestKey(request.Id), async (ct) =>
            {
                return await unitOfWork.Repository<Request>()
                .FirstOrDefaultAsync(
                    predicate: x => x.CreatedById == currentUser.UserId
                                && x.Id == request.Id,
                    selector: x => new RequestResponseDto
                    {
                        Id = x.Id,
                        DeliveryAddress = x.DeliveryAddress,
                        PickupAddress = x.PickupAddress,
                        Title = x.Title,
                        CreatedDate = x.CreatedDate,
                        DeliveredDate = x.DeliveredDate,
                        ReturnedReason = x.ReturnedReason,
                        StatusId = x.StatusId,
                        UpdatedById = x.UpdatedById,
                        UpdatedDate = x.UpdatedDate,
                        CreatedById = x.CreatedById,
                    });
            });

            return new ResultModel<RequestResponseDto>(result);
        }
    }
}
