using EventBus.ServiceBus;
using EventBus.ServiceBus.Configuration;
using EventContracts.DeliveryRequests.V1;
using Infrastructure;
using Infrastructure.Caching;
using Infrastructure.Outbox;
using Mediator.Extensions;
using Microsoft.EntityFrameworkCore;
using Report.Api.Consumers;
using Report.Application;
using Report.Infrastructure.Data;

namespace Report.Api.Extensions;

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

        services.AddHybridCacheManager(configuration, instanceName: "report:");
        return services;
    }

    private static IServiceCollection AddAppMediator(this IServiceCollection services)
    {
        services.AddMediator(typeof(Anchor).Assembly);
        return services;
    }

    private static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ReportDbContext>((sp, options) =>
            options
                .UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddEfUnitOfWork<ReportDbContext>();

        return services;
    }

    private static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusConfiguration = configuration
            .GetSection("ServiceBusConfiguration")
            .Get<ServiceBusConfiguration>()
            ?? throw new InvalidOperationException("Missing 'ServiceBusConfiguration' section.");

        services.AddServiceBusEventBus(serviceBusConfiguration)
            .AddTopicHandler<DeliveryRequestChangedEvent, DeliveryRequestChangedEventHandler>(serviceBusConfiguration.Topics["DeliveryRequestChangedTopic"]);

        services.AddInMemoryOutbox();

        return services;
    }
}
