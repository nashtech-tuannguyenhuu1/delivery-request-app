using System.Linq.Expressions;
using Core.Domain;

namespace Core.Data;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
    /// <summary>Composable query root. Use for custom LINQ; no-tracking by default.</summary>
    IQueryable<TEntity> Query(bool tracking = false);

    /// <summary>Get a single entity by its primary key value(s).</summary>
    Task<TEntity?> GetByKeyAsync(CancellationToken ct = default, params object[] keyValues);

    /// <summary>
    /// Get the first entity matching <paramref name="predicate"/>, or null.
    /// <paramref name="includes"/> are navigation paths to eager-load, e.g. "Items" or "Items.Product".
    /// </summary>
    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        IEnumerable<string>? includes = null,
        bool tracking = false,
        CancellationToken ct = default);

    /// <summary>
    /// Get all entities matching an optional <paramref name="predicate"/>, with optional ordering.
    /// <paramref name="includes"/> are navigation paths to eager-load, e.g. "Items" or "Items.Product".
    /// </summary>
    Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        IEnumerable<string>? includes = null,
        bool tracking = false,
        CancellationToken ct = default);

    /// <summary>
    /// Get a page of entities with optional filtering and ordering.
    /// <paramref name="includes"/> are navigation paths to eager-load, e.g. "Items" or "Items.Product".
    /// </summary>
    Task<PagedResult<TEntity>> PagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        IEnumerable<string>? includes = null,
        bool tracking = false,
        CancellationToken ct = default);

    /// <summary>Project the first entity matching <paramref name="predicate"/> into <typeparamref name="TResult"/>, or default.</summary>
    Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken ct = default);

    /// <summary>Project matching entities into <typeparamref name="TResult"/>, with optional filtering and ordering.</summary>
    Task<List<TResult>> FindAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken ct = default);

    /// <summary>Project a page of entities into <typeparamref name="TResult"/>, with optional filtering and ordering.</summary>
    Task<PagedResult<TResult>> PagedAsync<TResult>(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken ct = default);

    Task<List<TEntity>> ListAllAsync(CancellationToken ct = default);

    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);

    Task AddAsync(TEntity entity, CancellationToken ct = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    void RemoveRange(IEnumerable<TEntity> entities);
}
