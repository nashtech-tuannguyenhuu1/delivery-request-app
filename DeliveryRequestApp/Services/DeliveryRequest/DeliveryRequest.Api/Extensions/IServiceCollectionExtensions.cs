using DeliveryRequest.Application;
using DeliveryRequest.Infrastructure.Behaviors;
using DeliveryRequest.Infrastructure.Data;
using EventBus.ServiceBus;
using EventBus.ServiceBus.Configuration;
using EventContracts.Audits.V1;
using Infrastructure;
using Infrastructure.Audits;
using Infrastructure.Interceptors;
using Mediator.Abstractions;
using Mediator.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DeliveryRequest.Api.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddAppMediator();
        services.AddSwaggerGen();

        services.AddCurrentUser();
        services.AddDatabaseContext(configuration);
        services.AddEventBus(configuration);

        return services;
    }

    private static IServiceCollection AddAppMediator(this IServiceCollection services)
    {
        services.AddMediator(typeof(Anchor).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return services;
    }

    private static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuditInterceptor();
        services.AddAuditTrailInterceptor();
        services.AddDbContext<DeliveryRequestDbContext>((sp, options) =>
            options
                .UseSqlServer(configuration.GetConnectionString("Default"))
                .AddInterceptors(
                    sp.GetRequiredService<AuditSaveChangesInterceptor>(),
                    sp.GetRequiredService<AuditTrailInterceptor>()
                ));

        services.AddEfUnitOfWork<DeliveryRequestDbContext>();

        services.AddScoped<IAuditEventStore, InMemoryAuditEventStore>();

        return services;
    }

    private static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusConfiguration = configuration
            .GetSection("ServiceBusConfiguration")
            .Get<ServiceBusConfiguration>()
            ?? throw new InvalidOperationException("Missing 'ServiceBusConfiguration' section.");

        services.AddServiceBusEventBus(serviceBusConfiguration)
            .MapQueueMessage<EntityChangedEvent>(serviceBusConfiguration.Queues["Audit"]);

        return services;
    }
}
