using Audit.Application.UseCases.Queries;
using Mediator.Abstractions;

namespace Audit.Api.Routes.V1;

public static class AuditEndpointsV1
{
    public static void MapAuditEndpointsV1(this WebApplication app)
    {
        var group = app.MapGroup("/v1/audits");

        group.MapGet("/", async (string primaryKey, string? tableName, ISender sender) =>
        {
            return await sender.SendAsync(new GetAuditsByPrimaryKey.Query(primaryKey, tableName));
        });
    }
}
