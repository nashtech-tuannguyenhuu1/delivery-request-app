using AppContracts.DeliveryRequests.V1;
using Core.Data;
using Core.Domain;
using Core.Outbox;
using DeliveryRequest.Application.Entities;
using EventContracts.Audits.V1;
using Mediator.Abstractions;

namespace DeliveryRequest.Application.UseCases.Commands;

public class CreateRequest
{
    public record Command(string Title, string PickupAddress, string DeliveryAddress) : ICommand<Guid>;

    internal class Handler(IUnitOfWork unitOfWork, IOutboxStore outbox) : IRequestHandler<Command, ResultModel<Guid>>
    {
        public async Task<ResultModel<Guid>> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            var entity = new Request
            {
                // Client-generated so the key exists before SaveChanges (captured by the audit trail).
                Id = Guid.NewGuid(),
                Title = request.Title,
                PickupAddress = request.PickupAddress,
                DeliveryAddress = request.DeliveryAddress,
                StatusId = (int)RequestStatus.New,
            };

            await unitOfWork.Repository<Request>().AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            outbox.Enqueue(new DeliveryRequestChangedEvent
            {
                Id = entity.Id,
                Status = entity.StatusId,
            });

            return new ResultModel<Guid>(entity.Id);
        }
    }
}
