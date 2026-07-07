using DeliveryRequest.Api.Routes.V1;
using Mediator.Abstractions;

namespace DeliveryRequest.Api.Routes;

public static class DeliveryRequestEndpoints
{
    public static void MapDeliveryRequestEndpoints(this WebApplication app)
    {
        app.MapRequestsEndpointsV1();
    }
}
