using AppContracts.DeliveryRequests.V1.Responses;
using Core.Data;
using Core.Domain;
using DeliveryRequest.Application.Entities;
using Mediator.Abstractions;

namespace DeliveryRequest.Application.UseCases.Queries;

public class GetRequests
{
    public record Query : IQuery<IEnumerable<RequestResponseDto>>;

    internal class Handler(IUnitOfWork unitOfWork, ICurrentUser currentUser) : IRequestHandler<Query, ResultModel<IEnumerable<RequestResponseDto>>>
    {
        public async Task<ResultModel<IEnumerable<RequestResponseDto>>> HandleAsync(Query request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Repository<Request>()
                .FindAsync(predicate: x => x.CreatedById == currentUser.UserId,
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

            return new ResultModel<IEnumerable<RequestResponseDto>>(result);
        }
    }
}
