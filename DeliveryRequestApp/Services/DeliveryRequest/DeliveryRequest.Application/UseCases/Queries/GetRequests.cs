using AppContracts.DeliveryRequests.V1;
using AppContracts.DeliveryRequests.V1.Responses;
using Core.Data;
using Core.Domain;
using DeliveryRequest.Application.Entities;
using Mediator.Abstractions;

namespace DeliveryRequest.Application.UseCases.Queries;

public class GetRequests
{
    public record Query(RequestStatus? Status, int PageNumber = 1, int PageSize = 20) : IQuery<PagedResult<RequestResponseDto>>;

    internal class Handler(IUnitOfWork unitOfWork, ICurrentUser currentUser) : IRequestHandler<Query, ResultModel<PagedResult<RequestResponseDto>>>
    {
        public async Task<ResultModel<PagedResult<RequestResponseDto>>> HandleAsync(Query request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Repository<Request>().PagedAsync(
                request.PageNumber,
                request.PageSize,
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
                },
                predicate: x => x.CreatedById == currentUser.UserId
                    && (!request.Status.HasValue || x.StatusId == (int)request.Status.Value),
                orderBy: q => q.OrderByDescending(x => x.CreatedDate),
                ct: cancellationToken);

            return new ResultModel<PagedResult<RequestResponseDto>>(result);
        }
    }
}

