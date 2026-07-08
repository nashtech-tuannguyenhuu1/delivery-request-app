using Core.Data;
using Core.Domain;
using DeliveryRequest.Application.Entities;
using EventBus.Outbox;
using EventContracts.DeliveryRequests.V1;
using Mediator.Abstractions;

namespace DeliveryRequest.Application.UseCases.Commands;

public class DeleteRequest
{
    public record Command(Guid Id) : ICommand<bool>;

    internal class Handler(IUnitOfWork unitOfWork, ICurrentUser currentUser, IOutboxStore outbox) : IRequestHandler<Command, ResultModel<bool>>
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

            entity.DeletedById = currentUser.UserId;
            entity.DeletedDate = DateTimeOffset.Now;

            unitOfWork.Repository<Request>().Update(entity);

            outbox.Enqueue(new DeliveryRequestChangedEvent
            {
                Id = entity.Id,
                Status = entity.StatusId,
            });

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResultModel<bool>(true);
        }
    }
}
