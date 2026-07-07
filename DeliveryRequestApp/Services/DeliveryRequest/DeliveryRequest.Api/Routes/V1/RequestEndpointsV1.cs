using AppContracts.DeliveryRequests.V1;
using AppContracts.DeliveryRequests.V1.Requests;
using DeliveryRequest.Application.UseCases.Commands;
using DeliveryRequest.Application.UseCases.Queries;
using Mediator.Abstractions;

namespace DeliveryRequest.Api.Routes.V1;

public static class RequestEndpointsV1
{
    public static void MapRequestsEndpointsV1(this WebApplication app)
    {
        var group = app.MapGroup("/v1/requests");

        group.MapGet("/", async (ISender sender, RequestStatus? status, int page = 1, int pageSize = 20) =>
        {
            return await sender.SendAsync(new GetRequests.Query(status, page, pageSize));
        });

        group.MapGet("/{id}", async (Guid id, ISender sender) =>
        {
            return await sender.SendAsync(new GetRequestById.Query(id));
        });

        group.MapPost("/", async (CreateDeliveryRequestDto dto, ISender sender) =>
        {
            return await sender.SendAsync(new CreateRequest.Command(dto.Title, dto.PickupAddress, dto.DeliveryAddress));
        });

        group.MapPut("/{id}", async (Guid id, UpdateDeliveryRequestDto dto, ISender sender) =>
        {
            return await sender.SendAsync(new UpdateRequest.Command(id, dto.Title, dto.PickupAddress, dto.DeliveryAddress));
        });

        group.MapPatch("/{id}/status", async (Guid id, UpdateRequestStatusDto dto, ISender sender) =>
        {
            return await sender.SendAsync(new UpdateRequestStatus.Command(id, dto.Status, dto.Reason));
        });

        group.MapDelete("/{id}", async (Guid id, ISender sender) =>
        {
            return await sender.SendAsync(new DeleteRequest.Command(id));
        });
    }
}
