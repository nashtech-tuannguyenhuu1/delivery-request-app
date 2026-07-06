namespace EventBus;

/// <summary>
/// Enriches the headers of every outgoing envelope before it is serialized —
/// e.g. stamping the sending service's name. All registered providers run on
/// each publish; later providers and explicit PublishOptions headers may overwrite.
/// </summary>
public interface IPublishHeaderProvider
{
    void Enrich(IDictionary<string, string> headers);
}
