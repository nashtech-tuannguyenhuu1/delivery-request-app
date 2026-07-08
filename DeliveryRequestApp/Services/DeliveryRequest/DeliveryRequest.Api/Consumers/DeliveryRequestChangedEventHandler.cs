using DeliveryRequest.Application.UseCases.Queries;
using EventBus;
using EventContracts.Audits.V1;
using Mediator.Abstractions;

namespace DeliveryRequest.Api.Consumers;

public class DeliveryRequestChangedEventHandler(ISender sender) : IMessageHandler<DeliveryRequestChangedEvent>
{
    public async Task HandleAsync(MessageEnvelope<DeliveryRequestChangedEvent> envelope, CancellationToken cancellationToken)
    {
        await sender.SendAsync(new InvalidateRequestCache.Query(envelope.Content.Id));
    }
}
