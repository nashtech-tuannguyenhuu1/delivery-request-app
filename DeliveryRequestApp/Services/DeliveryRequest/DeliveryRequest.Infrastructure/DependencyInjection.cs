using Infrastructure;
using Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryRequest.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDeliveryRequestInfrastructure(
        this IServiceCollection services, string connectionString)
    {
        services.AddCurrentUser();
        services.AddAuditInterceptor();
        services.AddAuditTrailInterceptor();
        services.AddDbContext<DeliveryRequestDbContext>((sp, options) =>
            options
                .UseSqlServer(connectionString)
                .AddInterceptors(
                    sp.GetRequiredService<AuditSaveChangesInterceptor>(),
                    sp.GetRequiredService<AuditTrailInterceptor>()
                ));

        services.AddEfUnitOfWork<DeliveryRequestDbContext>();

        return services;
    }
}
