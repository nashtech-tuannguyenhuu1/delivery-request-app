using System.Linq.Expressions;
using Core.Data;
using Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

/// <summary>
/// Common EF Core implementation of <see cref="IRepository{TEntity}"/>.
/// Persistence-agnostic: it works against the injected <see cref="DbContext"/> only,
/// so no provider-specific code leaks in here. SaveChanges is intentionally owned by
/// the unit of work, not the repository.
/// </summary>
public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> Set;

    public EfRepository(DbContext context)
    {
        Context = context;
        Set = context.Set<TEntity>();
    }

    public IQueryable<TEntity> Query(bool tracking = false)
        => tracking ? Set : Set.AsNoTracking();

    public async Task<TEntity?> GetByKeyAsync(CancellationToken ct = default, params object[] keyValues)
        => await Set.FindAsync(keyValues, ct);

    public Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        IEnumerable<string>? includes = null,
        bool tracking = false,
        CancellationToken ct = default)
        => BuildQuery(predicate, orderBy: null, includes, tracking).FirstOrDefaultAsync(ct);

    public Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        IEnumerable<string>? includes = null,
        bool tracking = false,
        CancellationToken ct = default)
        => BuildQuery(predicate, orderBy, includes, tracking).ToListAsync(ct);

    public async Task<PagedResult<TEntity>> PagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        IEnumerable<string>? includes = null,
        bool tracking = false,
        CancellationToken ct = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 1;

        var query = BuildQuery(predicate, orderBy, includes, tracking);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<TEntity>(items, totalCount, pageNumber, pageSize);
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken ct = default)
        => BuildQuery(predicate, orderBy: null, includes: null, tracking: false)
            .Select(selector)
            .FirstOrDefaultAsync(ct);

    public Task<List<TResult>> FindAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken ct = default)
        => BuildQuery(predicate, orderBy, includes: null, tracking: false)
            .Select(selector)
            .ToListAsync(ct);

    public async Task<PagedResult<TResult>> PagedAsync<TResult>(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken ct = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 1;

        var query = BuildQuery(predicate, orderBy, includes: null, tracking: false);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(selector)
            .ToListAsync(ct);

        return new PagedResult<TResult>(items, totalCount, pageNumber, pageSize);
    }

    public Task<List<TEntity>> ListAllAsync(CancellationToken ct = default)
        => Set.AsNoTracking().ToListAsync(ct);

    public Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
        => predicate is null ? Set.CountAsync(ct) : Set.CountAsync(predicate, ct);

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
        => predicate is null ? Set.AnyAsync(ct) : Set.AnyAsync(predicate, ct);

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
        => await Set.AddAsync(entity, ct);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
        => await Set.AddRangeAsync(entities, ct);

    public void Update(TEntity entity) => Set.Update(entity);

    public void Remove(TEntity entity) => Set.Remove(entity);

    public void RemoveRange(IEnumerable<TEntity> entities) => Set.RemoveRange(entities);

    private IQueryable<TEntity> BuildQuery(
        Expression<Func<TEntity, bool>>? predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
        IEnumerable<string>? includes,
        bool tracking)
    {
        IQueryable<TEntity> query = tracking ? Set : Set.AsNoTracking();

        if (includes is not null)
        {
            foreach (var include in includes)
            {
                if (!string.IsNullOrWhiteSpace(include))
                {
                    query = query.Include(include);
                }
            }
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        return query;
    }
}
