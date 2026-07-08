using Audit.Application.Entities;
using Audit.Application.UseCases.Commands;
using EventBus;
using EventContracts.Audits.V1;
using Mediator.Abstractions;
using AuditEntity = Audit.Application.Entities.Audit;

namespace Audit.Api.Consumers;

public class EntityChangedEventHandler(ISender sender) : IMessageHandler<EntityChangedEvent>
{
    public async Task HandleAsync(MessageEnvelope<EntityChangedEvent> envelope, CancellationToken cancellationToken)
    {
        var e = envelope.Content;

        var audit = new AuditEntity
        {
            TableName = e.TableName,
            Action = e.Action,
            Timestamp = e.Timestamp,
            UserId = e.UserId,
            PrimaryKey = e.PrimaryKey,
            TransactionId = e.TransactionId,
            CorrelationId = e.CorrelationId,
            AuditProperties = e.Properties.Select(p => new AuditData
            {
                PropertyName = p.PropertyName,
                OldValue = p.OldValue,
                NewValue = p.NewValue,
            }).ToList(),
        };

        await sender.SendAsync(new SaveAuditData.Command(audit), cancellationToken);
    }
}
