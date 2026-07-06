namespace EventBus.ServiceBus;

/// <summary>Maps message types to the queue or topic they are published to.</summary>
public sealed class PublishRouteRegistry
{
    private readonly Dictionary<Type, string> _routes = [];

    internal void Add(Type messageType, string entityName) => _routes[messageType] = entityName;

    public string? Resolve(Type messageType) => _routes.GetValueOrDefault(messageType);
}
