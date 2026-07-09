using Audit.Api.Routes.V1;

namespace Audit.Api.Routes;

public static class AuditEndpoints
{
    public static void MapAuditEndpoints(this WebApplication app)
    {
        app.MapAuditEndpointsV1();
    }
}
