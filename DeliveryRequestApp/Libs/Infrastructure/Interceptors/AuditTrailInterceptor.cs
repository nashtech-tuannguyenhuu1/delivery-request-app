using Core.Data;
using EventContracts.Audits.V1;
using Infrastructure.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Interceptors;

public class AuditTrailInterceptor : SaveChangesInterceptor
{
    private readonly IAuditEventStore _auditEventStore;

    private readonly ICurrentUser _currentUserContext;

    private readonly HashSet<string> _ignoredProperties = new HashSet<string>
    {
        "CreatedDate",
        "CreatedById",
        "UpdatedDate",
        "UpdatedById"
    };

    public AuditTrailInterceptor(IAuditEventStore auditEventStore, ICurrentUser currentUserContext)
    {
        _auditEventStore = auditEventStore;
        _currentUserContext = currentUserContext;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
       DbContextEventData eventData,
       InterceptionResult<int> result,
       CancellationToken cancellationToken = default
    )
    {
        var context = eventData.Context;
        if (context == null) return result;

        Guid? transactionId = null;

        if (context.Database.CurrentTransaction != null)
        {
            transactionId = context.Database.CurrentTransaction.TransactionId;
        }

        foreach (var entry in context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified
                        || e.State == EntityState.Added
                        || e.State == EntityState.Deleted))
        {
            var audit = new EntityChangedEvent
            {
                TableName = entry.Entity.GetType().Name,
                Action = entry.State.ToString(),
                Timestamp = DateTime.UtcNow,
                UserId = _currentUserContext.UserId.ToString(),
                PrimaryKey = GetPrimaryKey(entry),
                TransactionId = transactionId,
            };

            foreach (var prop in entry.Properties)
            {
                if (_ignoredProperties.Contains(prop.Metadata.Name))
                    continue;

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified && !prop.IsModified)
                    continue;

                audit.Properties.Add(new EntityChangedEvent.PropertyDataDto
                {
                    PropertyName = prop.Metadata.Name,
                    OldValue = entry.State != EntityState.Added ? prop.OriginalValue?.ToString() : null,
                    NewValue = entry.State != EntityState.Deleted ? prop.CurrentValue?.ToString() : null
                });
            }

            _auditEventStore.Add(audit);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private string? GetPrimaryKey(EntityEntry entry)
    {
        var pk = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
        return pk?.CurrentValue?.ToString();
    }
}
