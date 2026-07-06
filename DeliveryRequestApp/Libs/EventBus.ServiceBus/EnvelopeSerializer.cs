using System.Text.Json;

namespace EventBus.ServiceBus;

internal static class EnvelopeSerializer
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
}
