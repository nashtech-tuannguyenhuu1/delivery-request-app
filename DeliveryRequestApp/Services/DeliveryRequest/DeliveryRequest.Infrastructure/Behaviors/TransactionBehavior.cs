using Core.Domain;
using DeliveryRequest.Infrastructure.Data;
using EventBus;
using Infrastructure.Audits;
using Mediator.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DeliveryRequest.Infrastructure.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    where TResponse : class
{
    private readonly DeliveryRequestDbContext _dbContext;

    private readonly IEventBus _eventBus;

    private readonly IAuditEventStore _auditEventStore;

    public TransactionBehavior(DeliveryRequestDbContext dbContext, IEventBus eventBus, IAuditEventStore auditEventStore)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _auditEventStore = auditEventStore;
    }

    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ITransactionRequest)
        {
            return await next();
        }

        if (_dbContext.Database.CurrentTransaction != null)
        {
            return await next();
        }

        var strategy = _dbContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            // Achieving atomicity
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            var response = await next();

            await PublishAuditEventsAsync();

            await transaction.CommitAsync(cancellationToken);

            return response;
        });
    }

    private async Task PublishAuditEventsAsync()
    {
        var events = (_auditEventStore.GetAll()).ToList();

        foreach (var e in events)
        {
            await _eventBus.PublishAsync(e);
        }

        _auditEventStore.Clear();
    }
}
