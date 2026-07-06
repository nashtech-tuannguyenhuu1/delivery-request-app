namespace EventBus.ServiceBus;

/// <summary>Subscription == null means a queue, otherwise topic/subscription.</summary>
internal sealed record EntityPath(string Name, string? Subscription)
{
    public override string ToString() => Subscription is null ? Name : $"{Name}/{Subscription}";
}

internal sealed record HandlerRegistration(EntityPath Entity, IMessageHandlerInvoker Invoker);
