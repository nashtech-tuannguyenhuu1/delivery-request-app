using EventBus;
using EventContracts.DeliveryRequests.V1;
using Mediator.Abstractions;
using Report.Application.UseCases.Commands;

namespace Report.Api.Consumers;

public class DeliveryRequestChangedEventHandler(ISender sender) : IMessageHandler<DeliveryRequestChangedEvent>
{
    public async Task HandleAsync(MessageEnvelope<DeliveryRequestChangedEvent> envelope, CancellationToken cancellationToken)
    {
        await sender.SendAsync(new UpdateReport.Command(envelope.Content.Id, envelope.Content.Status));
    }
}
