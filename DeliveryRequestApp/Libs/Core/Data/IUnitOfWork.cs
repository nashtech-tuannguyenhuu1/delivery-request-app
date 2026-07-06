using Core.Domain;

namespace Core.Data;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity;

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
