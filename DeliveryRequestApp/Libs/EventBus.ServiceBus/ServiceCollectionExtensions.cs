using Azure.Messaging.ServiceBus;
using EventBus.ServiceBus.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.ServiceBus;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Service Bus IEventBus and the consumer hosted service.
    /// Bind <paramref name="configuration"/> from appsettings, e.g.
    /// <c>builder.Configuration.GetSection("ServiceBusConfiguration").Get&lt;ServiceBusConfiguration&gt;()</c>.
    /// </summary>
    public static EventBusBuilder AddServiceBusEventBus(
        this IServiceCollection services, ServiceBusConfiguration configuration)
    {
        configuration.Validate();

        var routes = new PublishRouteRegistry();

        services.AddSingleton(configuration);
        services.AddSingleton(routes);
        services.AddSingleton(_ => new ServiceBusClient(configuration.AuthenticationDto.ConnectionString));
        services.AddSingleton<ServiceBusSenderRegistry>();
        // Exposes the terminal send for infrastructure that bypasses the pipeline (e.g. an outbox relay).
        services.AddSingleton<IOutboundSender>(sp => sp.GetRequiredService<ServiceBusSenderRegistry>());
        // Scoped so publish behaviours can use request-scoped services.
        services.AddScoped<IEventBus, ServiceBusEventBus>();
        // No-ops when zero handlers are registered, so publish-only services use the same call.
        services.AddHostedService<ServiceBusConsumerHostedService>();

        return new EventBusBuilder(services, routes);
    }
}

/// <summary>
/// Fluent registration surface. Pass the QueueDto/TopicDto from your bound
/// ServiceBusConfiguration, e.g. <c>configuration.Queues["Orders"]</c>.
/// </summary>
public sealed class EventBusBuilder
{
    private readonly IServiceCollection _services;
    private readonly PublishRouteRegistry _routes;

    internal EventBusBuilder(IServiceCollection services, PublishRouteRegistry routes)
    {
        _services = services;
        _routes = routes;
    }

    /// <summary>Publish messages of type <typeparamref name="T"/> to the given queue.</summary>
    public EventBusBuilder MapQueueMessage<T>(QueueDto queue) where T : class
    {
        _routes.Add(typeof(T), GetQueueName(queue));
        return this;
    }

    /// <summary>Publish messages of type <typeparamref name="T"/> to the given topic.</summary>
    public EventBusBuilder MapTopicMessage<T>(TopicDto topic) where T : class
    {
        _routes.Add(typeof(T), GetTopicName(topic));
        return this;
    }

    /// <summary>Consume messages of type <typeparamref name="TMessage"/> from the given queue.</summary>
    public EventBusBuilder AddQueueHandler<TMessage, THandler>(QueueDto queue)
        where TMessage : class
        where THandler : class, IMessageHandler<TMessage>
    {
        return AddHandler<TMessage, THandler>(new EntityPath(GetQueueName(queue), null));
    }

    /// <summary>
    /// Consume messages of type <typeparamref name="TMessage"/> from the given topic's
    /// subscription (TopicDto.SubscriptionName is required).
    /// </summary>
    public EventBusBuilder AddTopicHandler<TMessage, THandler>(TopicDto topic)
        where TMessage : class
        where THandler : class, IMessageHandler<TMessage>
    {
        var topicName = GetTopicName(topic);
        if (string.IsNullOrWhiteSpace(topic.SubscriptionName))
        {
            throw new InvalidOperationException(
                $"Topic '{topicName}' has no SubscriptionName configured; a subscription is required to consume.");
        }

        return AddHandler<TMessage, THandler>(new EntityPath(topicName, topic.SubscriptionName));
    }

    /// <summary>
    /// Registers an open-generic pipeline behaviour applied to every message type,
    /// e.g. <c>AddPipelineBehaviour(typeof(LoggingBehaviour&lt;&gt;))</c>.
    /// Behaviours run in registration order (first registered is outermost).
    /// </summary>
    public EventBusBuilder AddPipelineBehaviour(Type openGenericBehaviourType)
    {
        ArgumentNullException.ThrowIfNull(openGenericBehaviourType);
        if (!openGenericBehaviourType.IsGenericTypeDefinition)
        {
            throw new ArgumentException(
                $"'{openGenericBehaviourType}' must be an open generic type, e.g. typeof(LoggingBehaviour<>).",
                nameof(openGenericBehaviourType));
        }

        _services.AddScoped(typeof(IMessagePipelineBehaviour<>), openGenericBehaviourType);
        return this;
    }

    /// <summary>Registers a pipeline behaviour for a single message type <typeparamref name="TMessage"/>.</summary>
    public EventBusBuilder AddPipelineBehaviour<TMessage, TBehaviour>()
        where TMessage : class
        where TBehaviour : class, IMessagePipelineBehaviour<TMessage>
    {
        _services.AddScoped<IMessagePipelineBehaviour<TMessage>, TBehaviour>();
        return this;
    }

    /// <summary>
    /// Registers an open-generic publish behaviour applied to every message type,
    /// e.g. <c>AddPublishBehaviour(typeof(EnrichBehaviour&lt;&gt;))</c>.
    /// Behaviours run in registration order (first registered is outermost).
    /// </summary>
    public EventBusBuilder AddPublishBehaviour(Type openGenericBehaviourType)
    {
        ArgumentNullException.ThrowIfNull(openGenericBehaviourType);
        if (!openGenericBehaviourType.IsGenericTypeDefinition)
        {
            throw new ArgumentException(
                $"'{openGenericBehaviourType}' must be an open generic type, e.g. typeof(EnrichBehaviour<>).",
                nameof(openGenericBehaviourType));
        }

        _services.AddScoped(typeof(IPublishBehaviour<>), openGenericBehaviourType);
        return this;
    }

    /// <summary>Registers a publish behaviour for a single message type <typeparamref name="TMessage"/>.</summary>
    public EventBusBuilder AddPublishBehaviour<TMessage, TBehaviour>()
        where TMessage : class
        where TBehaviour : class, IPublishBehaviour<TMessage>
    {
        _services.AddScoped<IPublishBehaviour<TMessage>, TBehaviour>();
        return this;
    }

    private EventBusBuilder AddHandler<TMessage, THandler>(EntityPath entity)
        where TMessage : class
        where THandler : class, IMessageHandler<TMessage>
    {
        // Keyed by the full message type name — matches the key used at dispatch
        // in MessageHandlerInvoker<TMessage>.
        _services.AddKeyedScoped<IMessageHandler<TMessage>, THandler>(typeof(TMessage).FullName);
        _services.AddSingleton(new HandlerRegistration(entity, new MessageHandlerInvoker<TMessage>()));
        return this;
    }

    private static string GetQueueName(QueueDto? queue)
    {
        if (queue is null || string.IsNullOrWhiteSpace(queue.QueueName))
        {
            throw new InvalidOperationException(
                "A QueueDto with a non-empty QueueName is required. " +
                "Pass one from ServiceBusConfiguration.Queues, e.g. configuration.Queues[\"Orders\"].");
        }

        return queue.QueueName;
    }

    private static string GetTopicName(TopicDto? topic)
    {
        if (topic is null || string.IsNullOrWhiteSpace(topic.TopicName))
        {
            throw new InvalidOperationException(
                "A TopicDto with a non-empty TopicName is required. " +
                "Pass one from ServiceBusConfiguration.Topics, e.g. configuration.Topics[\"UserEvents\"].");
        }

        return topic.TopicName;
    }
}
