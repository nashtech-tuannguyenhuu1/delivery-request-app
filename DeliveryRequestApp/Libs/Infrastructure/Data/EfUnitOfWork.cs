using Core.Data;
using Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Data;

/// <summary>
/// Common EF Core implementation of <see cref="IUnitOfWork"/>.
/// Generic over the concrete <typeparamref name="TContext"/> so a single implementation
/// serves any service's DbContext. Repositories are lazily created and cached per instance.
/// </summary>
public class EfUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    public EfUnitOfWork(TContext context)
    {
        _context = context;
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity
    {
        if (_repositories.TryGetValue(typeof(TEntity), out var existing))
        {
            return (IRepository<TEntity>)existing;
        }

        var repository = new EfRepository<TEntity>(_context);
        _repositories[typeof(TEntity)] = repository;
        return repository;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        GC.SuppressFinalize(this);
    }
}
