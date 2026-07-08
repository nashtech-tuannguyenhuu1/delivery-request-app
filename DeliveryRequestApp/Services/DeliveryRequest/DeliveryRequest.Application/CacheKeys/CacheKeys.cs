namespace DeliveryRequest.Application.CacheKeys;
public static class CacheKeys
{
    public static string GetRequestKey(Guid id) => $"request-{id}";
}
