using Audit.Api.Consumers;
using Audit.Application;
using Audit.Infrastructure.Data;
using EventBus.ServiceBus;
using EventBus.ServiceBus.Configuration;
using EventContracts.Audits.V1;
using Infrastructure;
using Mediator.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Audit.Api.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddAppMediator();
        services.AddDatabaseContext(configuration);
        services.AddEventBus(configuration);

        return services;
    }

    private static IServiceCollection AddAppMediator(this IServiceCollection services)
    {
        services.AddMediator(typeof(Anchor).Assembly);

        return services;
    }

    private static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuditDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddEfUnitOfWork<AuditDbContext>();

        return services;
    }

    private static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusConfiguration = configuration
            .GetSection("ServiceBusConfiguration")
            .Get<ServiceBusConfiguration>()
            ?? throw new InvalidOperationException("Missing 'ServiceBusConfiguration' section.");

        services.AddServiceBusEventBus(serviceBusConfiguration)
            .AddQueueHandler<EntityChangedEvent, EntityChangedEventHandler>(serviceBusConfiguration.Queues["Audit"]);

        return services;
    }
}
