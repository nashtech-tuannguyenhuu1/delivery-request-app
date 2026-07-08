using Core.Data;
using Infrastructure.Data;
using Infrastructure.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the common EF Core unit of work for the given <typeparamref name="TContext"/>.
    /// The <see cref="DbContext"/> itself must already be registered (e.g. via AddDbContext).
    /// Repositories are obtained through <see cref="IUnitOfWork.Repository{TEntity}"/>.
    /// </summary>
    public static IServiceCollection AddEfUnitOfWork<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddScoped<IUnitOfWork, EfUnitOfWork<TContext>>();
        return services;
    }

    /// <summary>
    /// Registers the audit interceptor. Add it to a context via
    /// <c>options.AddInterceptors(sp.GetRequiredService&lt;AuditSaveChangesInterceptor&gt;())</c>.
    /// An <see cref="ICurrentUser"/> must be registered (see <see cref="AddCurrentUser"/>).
    /// </summary>
    public static IServiceCollection AddAuditInterceptor(this IServiceCollection services)
    {
        services.AddScoped<AuditSaveChangesInterceptor>();
        return services;
    }

    /// <summary>
    /// Registers the scoped current-user holder (exposed as <see cref="ICurrentUser"/>) and the
    /// middleware that populates it. Call <see cref="UseCurrentUser"/> in the request pipeline.
    /// </summary>
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddScoped<CurrentUser>();
        services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<CurrentUser>());
        services.AddScoped<CurrentUserMiddleware>();
        return services;
    }

    /// <summary>Adds the middleware that reads the current user into <see cref="ICurrentUser"/>.</summary>
    public static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app)
        => app.UseMiddleware<CurrentUserMiddleware>();

    public static IServiceCollection AddAuditTrailInterceptor(this IServiceCollection services)
    {
        services.AddScoped<AuditTrailInterceptor>();
        return services;
    }
}
