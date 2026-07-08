using DeliveryRequest.Api.Routes.V1;

namespace DeliveryRequest.Api.Routes;

public static class DeliveryRequestEndpoints
{
    public static void MapDeliveryRequestEndpoints(this WebApplication app)
    {
        app.MapRequestsEndpointsV1();
        app.MapDocumentsEndpointsV1();
    }
}
