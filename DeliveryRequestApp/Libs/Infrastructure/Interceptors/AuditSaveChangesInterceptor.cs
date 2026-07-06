using Core.Data;
using Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Interceptors;

/// <summary>
/// EF Core interceptor that stamps audit fields (created/updated by + date) based on the
/// Core auditing interfaces just before changes are persisted. Reusable across any DbContext.
/// </summary>
public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser _currentUser;

    public AuditSaveChangesInterceptor(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAudit(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var userId = _currentUser.UserId;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity is ICreatableDateEntity created)
                    {
                        created.CreatedDate = now;
                    }
                    if (entry.Entity is ICreatableByEntity createdBy && userId.HasValue)
                    {
                        createdBy.CreatedById = userId.Value;
                    }
                    break;

                case EntityState.Modified:
                    if (entry.Entity is IUpdatableNullDateEntity updated)
                    {
                        updated.UpdatedDate = now;
                    }
                    if (entry.Entity is IUpdatableNullEntity updatedBy)
                    {
                        updatedBy.UpdatedById = userId;
                    }
                    break;
            }
        }
    }
}
