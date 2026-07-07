using DeliveryRequest.Application.UseCases.Queries;
using Mediator.Abstractions;

namespace DeliveryRequest.Api.Routes.V1;

public static class RequestEndpointsV1
{
    public static void MapRequestsEndpointsV1(this WebApplication app)
    {
        var group = app.MapGroup("/v1/requests");

        group.MapGet("/", async (ISender sender) =>
        {
            return await sender.SendAsync(new GetRequests.Query());
        });
    }
}
