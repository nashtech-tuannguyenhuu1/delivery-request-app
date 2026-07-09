using EventBus.Outbox;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Outbox;

public static class Extensions
{
    public static IServiceCollection AddInMemoryOutbox(this IServiceCollection services)
    {
        services.AddScoped<IOutboxStore, OutboxStore>();
        return services;
    }
}
